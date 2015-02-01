#region Dependencies

using System;
using System.Collections.Generic;
using System.Linq;
using MusicBeePlugin.AndroidRemote.Data;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.Text;

#endregion

namespace MusicBeePlugin.Modules
{
	public class PlaylistModule
	{
		private readonly CacheHelper _cHelper;
		private Plugin.MusicBeeApiInterface _api;

		public PlaylistModule(Plugin.MusicBeeApiInterface api, CacheHelper cHelper)
		{
			_api = api;
			_cHelper = cHelper;
		}

		public void StoreAvailablePlaylists()
		{
			var playlists = GetPlaylistsFromApi();
			string[] files = {};
			foreach (var playlist in playlists)
			{
				var path = playlist.Path;
				playlist.Name = _api.Playlist_GetName(path);
				_api.Playlist_QueryFilesEx(path, ref files);
				playlist.Tracks = files.Count();
			}

			using (var db = _cHelper.GetDbConnection())
			{
				db.SaveAll(GetNewPlaylists(playlists));
			}
		}

		/// <summary>
		/// Checks if two lists have the same tracks in the same order.
		/// </summary>
		/// <param name="firstList"></param>
		/// <param name="secondList"></param>
		/// <returns></returns>
		public bool CheckIfSame(IList<PlaylistTrackInfo> firstList, IList<PlaylistTrackInfo> secondList)
		{
			var same = true;

			if (firstList.Count != secondList.Count)
			{
				same = false;
			}
			else
			{
				for (var i = 0; i < firstList.Count; i++)
				{
					var equal = firstList[i].Equals(secondList[i]);
					if (!equal)
					{
						same = false;
						break;
						;
					}
				}
			}

			return same;
		}

		public object CheckPlaylistsForChanges()
		{
			var playlists = GetPlaylistsFromApi();

			foreach (var playlist in playlists)
			{
				CachePlaylistTracks(playlist.Path);
			}

			using (var db = _cHelper.GetDbConnection())
			{
				var cachedPlaylists = db.Select<Playlist>();
				var deleted = cachedPlaylists.Except(playlists).ToList();
				var added = playlists.Except(cachedPlaylists).ToList();
				var test = "C:\\Users\\developer\\Music\\MusicBee\\Playlists\\Playlist.mbp";


				var playlistTrackInfos = GetPlaylistTracks(test);
				var cachedPlaylistTracks = GetCachedPlaylistTracks(test);
				var same = CheckIfSame(playlistTrackInfos, cachedPlaylistTracks);


				return new
				{
					deleted,
					added,
					same
				};
			}
		}

		/// <summary>
		///     It takes a path to a playlist and returns a list with the info
		///     of the tracks in this playlist.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public List<PlaylistTrackInfo> GetPlaylistTracks(string path)
		{
			var list = new List<PlaylistTrackInfo>();
			var trackList = new string[] {};
			if (_api.Playlist_QueryFilesEx(path, ref trackList))
			{
				list.AddRange(trackList.Select(trackPath => new PlaylistTrackInfo
				{
					Path = trackPath,
					Artist = _api.Library_GetFileTag(trackPath, Plugin.MetaDataType.Artist),
					Title = _api.Library_GetFileTag(trackPath, Plugin.MetaDataType.TrackTitle)
				}));
			}
			;

			return list;
		}

		public List<PlaylistTrackInfo> GetCachedPlaylistTracks(string path)
		{
			var list = new List<PlaylistTrackInfo>();
			var playlistId = GetPlaylistId(path);
			using (var db = _cHelper.GetDbConnection())
			{
				var join = new JoinSqlBuilder<PlaylistTrackInfo, PlaylistTrack>();
				join = join.Join<PlaylistTrackInfo, PlaylistTrack>(pti => pti.Id, pt => pt.TrackInfoId)
					.Where<PlaylistTrack>(p => p.PlaylistId == playlistId)
					.OrderBy<PlaylistTrack>(pt => pt.Position);
				var sql = join.ToSql();
				var result = db.Query<PlaylistTrackInfo>(sql);
				list.AddRange(result);
			}

			return list;
		}

		/// <summary>
		///     Gets the id of the Playlist in the database using
		///     the path of the Playlist in the file system.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public long GetPlaylistId(string path)
		{
			long id = -1;
			using (var db = _cHelper.GetDbConnection())
			{
				var match = db.Select<Playlist>(p => p.Path.Contains(path));
				if (match != null && match.Count > 0)
				{
					id = match[0].Id;
				}
			}

			return id;
		}

		/// <summary>
		///     Gets a path of a playlist in the filesystem and stores
		///     the tracks in the playlist in the database
		/// </summary>
		/// <param name="path"></param>
		public void CachePlaylistTracks(string path)
		{
			var playlistId = GetPlaylistId(path);
			var db = _cHelper.GetDbConnection();
			var trackInfoCache = db.Select<PlaylistTrackInfo>();

			var transaction = db.BeginTransaction();
			var position = 0;
			foreach (var trackInfo in GetPlaylistTracks(path))
			{
				long id;
				if (trackInfoCache.Contains(trackInfo))
				{
					var info = trackInfoCache.Find(p => p.Path.Equals(trackInfo.Path));
					id = info.Id;
				}
				else
				{
					db.Save(trackInfo);
					id = db.GetLastInsertId();
				}

				var trackPlay = new PlaylistTrack
				{
					PlaylistId = playlistId,
					TrackInfoId = id,
					Position = position++
				};

				db.Save(trackPlay);
			}

			transaction.Commit();
		}

		private IEnumerable<Playlist> GetNewPlaylists(IEnumerable<Playlist> list)
		{
			var cachedLists = GetCachedPlaylists();
			var newPlaylists = list.Where(playlist =>
				!cachedLists.Exists(x =>
					x.Path == playlist.Path))
				.ToList();
			return newPlaylists;
		}

		private List<Playlist> GetPlaylistsFromApi()
		{
			_api.Playlist_QueryPlaylists();
			var playlists = new List<Playlist>();
			while (true)
			{
				var path = _api.Playlist_QueryGetNextPlaylist();
				var name = _api.Playlist_GetName(path);
				string[] tracks = {};
				_api.Playlist_QueryFilesEx(path, ref tracks);

				if (string.IsNullOrEmpty(path)) break;
				var playlist = new Playlist
				{
					Name = name,
					Path = path,
					Tracks = tracks.Count(),
				};
				playlists.Add(playlist);
			}
			return playlists;
		}

		private List<Playlist> GetCachedPlaylists()
		{
			using (var db = _cHelper.GetDbConnection())
			{
				return db.Select<Playlist>();
			}
		}

		public PaginatedResponse<Playlist> GetAvailablePlaylists(int limit = 50, int offset = 0)
		{
			var playlists = GetCachedPlaylists();
			var result = new PaginatedPlaylistResponse();
			result.CreatePage(limit, offset, playlists);
			return result;
		}

		public PaginatedResponse<PlaylistTrackInfo> GetPlaylistTracks(int id, int limit = 50, int offset = 0)
		{
			string[] pathList = {};
			var playlist = GetPlaylistById(id);
			var paginated = new PaginatedPlaylistTrackResponse();

			if (!_api.Playlist_QueryFilesEx(playlist.Path, ref pathList))
			{
				return paginated;
			}

			var index = 0;
			var playlistTracks = pathList.Select(path => new PlaylistTrackInfo
			{
				Artist = _api.Library_GetFileTag(path, Plugin.MetaDataType.Artist),
				Title = _api.Library_GetFileTag(path, Plugin.MetaDataType.TrackTitle),
				Path = path
			}).ToList();

			paginated.CreatePage(limit, offset, playlistTracks);
			return paginated;
		}

		/// <summary>
		///     Given the hash representing of a playlist it plays the specified playlist.
		/// </summary>
		/// <param name="path">The playlist path</param>
		public SuccessResponse PlaylistPlayNow(string path)
		{
			return new SuccessResponse
			{
				Success = _api.Playlist_PlayNow(path)
			};
		}

		public bool DeleteTrackFromPlaylist(int id, int index)
		{
			var playlist = GetPlaylistById(id);
			return _api.Playlist_RemoveAt(playlist.Path, index);
		}

		public SuccessResponse CreateNewPlaylist(string name, string[] list)
		{
			var path = _api.Playlist_CreatePlaylist(string.Empty, name, list);
			var playlist = new Playlist
			{
				Path = path,
				Name = name,
				Tracks = list.Count()
			};
			using (var db = _cHelper.GetDbConnection())
			{
				db.Save(playlist);
				return new SuccessResponse
				{
					Success = db.GetLastInsertId() > 0
				};
			}
		}

		public bool MovePlaylistTrack(int id, int from, int to)
		{
			var playlist = GetPlaylistById(id);
			int[] aFrom = {@from};
			int dIn;
			if (@from > to)
			{
				dIn = to - 1;
			}
			else
			{
				dIn = to;
			}

			return _api.Playlist_MoveFiles(playlist.Path, aFrom, dIn);
		}

		public bool PlaylistAddTracks(int id, string[] list)
		{
			var playlist = GetPlaylistById(id);
			return _api.Playlist_AppendFiles(playlist.Path, list);
		}

		public bool PlaylistDelete(int id)
		{
			var playlist = GetPlaylistById(id);
			using (var db = _cHelper.GetDbConnection())
			{
				db.DeleteById<Playlist>(id);
				return _api.Playlist_DeletePlaylist(playlist.Path);
			}
		}

		private Playlist GetPlaylistById(int id)
		{
			using (var db = _cHelper.GetDbConnection())
			{
				try
				{
					return db.GetById<Playlist>(id);
				}
				catch
				{
					throw HttpError.NotFound("Playlist resource with id {0} does not exist".Fmt(id));
				}
			}
		}
	}
}