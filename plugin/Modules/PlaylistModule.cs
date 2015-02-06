#region Dependencies

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MusicBeePlugin.AndroidRemote.Data;
using MusicBeePlugin.Comparers;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using NLog;
using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.Text;

#endregion

namespace MusicBeePlugin.Modules
{
	/// <summary>
	///     This module is responsible for the playlist functionality.
	///     It implements all the playlist operation with the MusicBee API and the
	///     plugin cache.
	/// </summary>
	public class PlaylistModule
	{
		/// <summary>
		///     The logger is used to log errors.
		/// </summary>
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		///     <see cref="CacheHelper" />
		/// </summary>
		private readonly CacheHelper _cHelper;

		/// <summary>
		///     MusicBee API Interface.
		/// </summary>
		private Plugin.MusicBeeApiInterface _api;

		/// <summary>
		///     Creates a new <see cref="PlaylistModule" />.
		/// </summary>
		/// <param name="api"></param>
		/// <param name="cHelper"></param>
		public PlaylistModule(Plugin.MusicBeeApiInterface api, CacheHelper cHelper)
		{
			_api = api;
			_cHelper = cHelper;
		}

		/// <summary>
		///     Syncs the playlist information in the cache with the information available
		///     from the MusicBee API.
		/// </summary>
		/// <returns></returns>
		public void SyncPlaylistsWithCache()
		{
			var playlists = GetPlaylistsFromApi();
			var cachedPlaylists = GetCachedPlaylists();

			var playlistComparer = new PlaylistComparer();
			var playlistsToInsert = playlists.Except(cachedPlaylists, playlistComparer).ToList();
			var playlistsToRemove = cachedPlaylists.Except(playlists, playlistComparer).ToList();

			foreach (var playlist in playlistsToRemove)
			{
				playlist.DateDeleted = DateTime.UtcNow;
				cachedPlaylists.Remove(playlist);

				var db = _cHelper.GetDbConnection();
				db.UpdateOnly(new PlaylistTrack {DateDeleted = DateTime.UtcNow},
					o => o.Update(p => p.DateDeleted)
						.Where(pl => pl.PlaylistId == playlist.Id));
				db.Dispose();
			}

			cachedPlaylists.AddRange(playlistsToInsert);

			using (var db = _cHelper.GetDbConnection())
			{
				db.SaveAll(playlistsToRemove);
			}

			foreach (var cachedPlaylist in cachedPlaylists)
			{
				SyncPlaylistDataWithCache(cachedPlaylist);
			}
		}

		/// <summary>
		///     Syncs the <see cref="PlaylistTrack" /> cache with the data available
		///     from the MusicBee API.
		/// </summary>
		/// <param name="playlist">The playlist for which the sync happens</param>
		private void SyncPlaylistDataWithCache(Playlist playlist)
		{
			using (var db = _cHelper.GetDbConnection())
			{
				var tracksUpdated = 0;
				var cachedTracks = db.Select<PlaylistTrack>(track => track.PlaylistId == playlist.Id);

				var playlistTracks = GetPlaylistTracksFromApi(playlist);
				var cachedPlaylistTracks = GetCachedPlaylistTracks(playlist);

				var comparer = new PlaylistTrackInfoComparer();

				var tracksToInsert = playlistTracks.Except(cachedPlaylistTracks, comparer).ToList();
				var tracksToDelete = cachedPlaylistTracks.Except(playlistTracks, comparer).ToList();

				var duplicatesPaths = tracksToDelete.GroupBy(x => x.Path)
					.Where(group => group.Count() > 1)
					.Select(group => group.Key)
					.ToList();


				foreach (var path in duplicatesPaths)
				{
					var duplicatesDeleted = tracksToDelete.FindAll(track => track.Path.Equals(path));

					foreach (var deleted in duplicatesDeleted)
					{
						var inserted = tracksToInsert.Find(track => track.Path.Equals(path));
						if (inserted == null) continue;

						tracksToDelete.Remove(deleted);
						tracksToInsert.Remove(inserted);
						var cached = cachedPlaylistTracks.Find(track => track.GetHashCode() == deleted.GetHashCode());
						cached.Position = inserted.Position;

						var updated = cachedTracks.Find(track => track.Id == cached.Id);

						updated.Position = cached.Position;
						updated.DateUpdated = DateTime.UtcNow;
						//Track has been updated so increment
						tracksUpdated++;
					}
				}
				// Important! Deactivating the Position inclusion from the comparer.
				// This will help us find the tracks that have been moved.
				comparer.IncludePosition = false;
				var commonElements = tracksToDelete.Intersect(tracksToInsert, comparer).ToList();
				foreach (var trackInfo in commonElements)
				{
					var track = tracksToInsert.Find(p => p.Path.Equals(trackInfo.Path));
					trackInfo.Position = track.Position;
					tracksToDelete.Remove(trackInfo);
					tracksToInsert.Remove(track);

					var updated = cachedTracks.Find(cTrack => cTrack.Id == trackInfo.Id);

					updated.Position = trackInfo.Position;
					updated.DateUpdated = DateTime.UtcNow;
					// Track has been updated so increment.
					tracksUpdated++;
				}

				// Reactivating
				comparer.IncludePosition = true;
				Logger.Info("{0} tracks inserted.\t {1} tracks deleted.\t {2} tracks updated.", tracksToInsert.Count(),
					tracksToDelete.Count(), tracksUpdated);

				foreach (var track in tracksToDelete)
				{
					var cachedTrack = cachedTracks.Find(t => t.PlaylistId == playlist.Id
					                                         && t.TrackInfoId == track.Id);
					cachedTrack.DateDeleted = DateTime.UtcNow;
					cachedPlaylistTracks.Remove(track);
				}

				var tiCache = db.Select<PlaylistTrackInfo>();

				using (var transaction = db.OpenTransaction())
				{
					foreach (var track in tracksToInsert)
					{
						StorePlaylistTrack(playlist, db, track, tiCache);
					}
					transaction.Commit();
				}

				cachedPlaylistTracks.Sort();

				Debug.WriteLine("The playlists should be equal now: {0}",
					playlistTracks.SequenceEqual(cachedPlaylistTracks, comparer));

				if (tracksToInsert.Count + tracksToDelete.Count + tracksUpdated > 0)
				{
					playlist.DateUpdated = DateTime.UtcNow;
					db.Save(playlist);
				}

				db.SaveAll(cachedTracks);
			}
		}

		/// <summary>
		///     Checks for unused <see cref="PlaylistTrackInfo" /> entries and sets
		///     the <see cref="TypeBase.DateDeleted" />property to the current UTC
		///     DateTime. />
		/// </summary>
		private void CleanUnusedTrackInfo()
		{
			using (var db = _cHelper.GetDbConnection())
			{
				var usedTrackInfoIds = db.Select<PlaylistTrack>()
					.Select(track => track.TrackInfoId)
					.ToList();

				var storedTrackInfoIds = db.Select<PlaylistTrackInfo>()
					.Select(track => track.Id)
					.ToList();

				var unused = storedTrackInfoIds.Except(usedTrackInfoIds);

				using (var transaction = db.OpenTransaction())
				{
					foreach (var id in unused)
					{
						db.UpdateOnly(new PlaylistTrackInfo {DateDeleted = DateTime.UtcNow}, o => o.Update(p => p.DateDeleted)
							.Where(p => p.Id == id));
					}
					transaction.Commit();
				}
			}
		}

		/// <summary>
		///     Caches a <see cref="PlaylistTrack" /> in the database along with the
		///     related <see cref="PlaylistTrackInfo" />. In case the information already
		///     exist in the cache it will use the existing entry.
		/// </summary>
		/// <param name="playlist">The playlist that contains the tracks.</param>
		/// <param name="db">A database connection.</param>
		/// <param name="track">The track that will be added to the database.</param>
		/// <param name="tiCache">A List containing the cached Playlist track metadata.</param>
		private static void StorePlaylistTrack(Playlist playlist, IDbConnection db, PlaylistTrackInfo track,
			List<PlaylistTrackInfo> tiCache)
		{
			long id;
			if (tiCache.Contains(track))
			{
				var info = tiCache.Find(p => p.Path.Equals(track.Path));
				id = info.Id;
				// If the entry was previously soft deleted now the entry will be
				// reused so we are remove the DateDeleted.
				if (info.DateDeleted != null)
				{
					info.DateDeleted = null;
					db.Save(info);
				}
			}
			else
			{
				db.Save(track);
				id = db.GetLastInsertId();
			}

			var trackPlay = new PlaylistTrack
			{
				PlaylistId = playlist.Id,
				TrackInfoId = id,
				Position = track.Position
			};

			db.Save(trackPlay);
		}

		/// <summary>
		///     Gets the PlaylistTracks from the MusicBee API for a specified
		///     <paramref name="playlist" />.
		/// </summary>
		/// <param name="playlist">
		///     A <c>playlist</c> for which we want to get the
		///     tracks from the api.
		/// </param>
		/// <returns>The List of tracks for the <paramref name="playlist" />.</returns>
		public List<PlaylistTrackInfo> GetPlaylistTracksFromApi(Playlist playlist)
		{
			var list = new List<PlaylistTrackInfo>();
			var trackList = new string[] {};
			if (_api.Playlist_QueryFilesEx(playlist.Path, ref trackList))
			{
				var position = 0;
				list.AddRange(trackList.Select(trackPath => new PlaylistTrackInfo
				{
					Path = trackPath,
					Artist = _api.Library_GetFileTag(trackPath, Plugin.MetaDataType.Artist),
					Title = _api.Library_GetFileTag(trackPath, Plugin.MetaDataType.TrackTitle),
					Position = position++
				}));
			}

			return list;
		}

		/// <summary>
		///     Gets the cached <c>playlist</c> tracks ordered by the position in the <c>playlist</c>.
		/// </summary>
		/// <param name="playlist"></param>
		/// <returns></returns>
		public List<PlaylistTrackInfo> GetCachedPlaylistTracks(Playlist playlist)
		{
			var list = new List<PlaylistTrackInfo>();

			using (var db = _cHelper.GetDbConnection())
			{
				var join = new JoinSqlBuilder<PlaylistTrackInfo, PlaylistTrack>();
				join = join.Join<PlaylistTrackInfo, PlaylistTrack>(pti => pti.Id, pt => pt.TrackInfoId,
					trackInfo => new
					{
						trackInfo.Path,
						trackInfo.Title,
						trackInfo.Id,
						trackInfo.Artist,
						trackInfo.DateAdded,
						trackInfo.DateUpdated
					}, playlistTrack => new {playlistTrack.Position})
					.Where<PlaylistTrack>(p => p.PlaylistId == playlist.Id && p.DateDeleted == null)
					.OrderBy<PlaylistTrack>(pt => pt.Position);
				var sql = join.ToSql();
				var result = db.Query<PlaylistTrackInfo>(sql);
				list.AddRange(result);
			}

			return list;
		}

		/// <summary>
		///     Retrieves the playlists from the MusicBee API.
		/// </summary>
		/// <returns>A list of <see cref="Playlist" /> objects.</returns>
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
					Tracks = tracks.Count()
				};
				playlists.Add(playlist);
			}
			return playlists;
		}

		/// <summary>
		///     Retrieves the playlists stored in the MusicBee Remote cache.
		/// </summary>
		/// <returns>A list of <see cref="Playlist" /> objects.</returns>
		private List<Playlist> GetCachedPlaylists()
		{
			using (var db = _cHelper.GetDbConnection())
			{
				return db.Select<Playlist>(pl => pl.DateDeleted == null);
			}
		}

		/// <summary>
		///     Retrieves a page of <see cref="Playlist" /> results.
		/// </summary>
		/// <param name="limit">The number of entries in the page.</param>
		/// <param name="offset">The index of the first result in the page.</param>
		/// <returns>A PaginatedResponse containing playlists</returns>
		public PaginatedResponse<Playlist> GetAvailablePlaylists(int limit = 50, int offset = 0)
		{
			var playlists = GetCachedPlaylists();
			var result = new PaginatedPlaylistResponse();
			result.CreatePage(limit, offset, playlists);
			return result;
		}

		/// <summary>
		///     Retrieves a page of <see cref="PlaylistTrackInfo" /> results.
		/// </summary>
		/// <param name="limit">The number of the results in the page.</param>
		/// <param name="offset">The index of the first result in the page.</param>
		/// <returns></returns>
		public PaginatedResponse<PlaylistTrackInfo> GetPlaylistTracksInfo(int limit = 50, int offset = 0)
		{
			List<PlaylistTrackInfo> trackInfo;
			using (var db = _cHelper.GetDbConnection())
			{
				trackInfo = db.Select<PlaylistTrackInfo>(ti => ti.DateDeleted == null);
			}
			var paginated = new PaginatedPlaylistTrackInfoResponse();
			paginated.CreatePage(limit, offset, trackInfo);
			return paginated;
		}

		/// <summary>
		///     Retrieves a page of <see cref="PlaylistTrack" /> results.
		/// </summary>
		/// <param name="id">The id of the playlist that contains the tracks.</param>
		/// <param name="limit">The number of the results in the page.</param>
		/// <param name="offset">The index of the first result in the page.</param>
		/// <returns></returns>
		public PaginatedResponse<PlaylistTrack> GetPlaylistTracks(int id, int limit = 50, int offset = 0)
		{
			var playlist = GetPlaylistById(id);
			List<PlaylistTrack> playlistTracks;
			using (var db = _cHelper.GetDbConnection())
			{
				playlistTracks = db.Select<PlaylistTrack>(pl => pl.PlaylistId == playlist.Id);
			}
			var paginated = new PaginatedPlaylistTrackResponse();
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

		/// <summary>
		///     Removes a track from the specified playlist.
		/// </summary>
		/// <param name="id">The <c>id</c> of the playlist</param>
		/// <param name="position">The <c>position</c> of the track in the playlist</param>
		/// <returns></returns>
		public bool DeleteTrackFromPlaylist(int id, int position)
		{
			var playlist = GetPlaylistById(id);
			var success = _api.Playlist_RemoveAt(playlist.Path, position);
			if (success)
			{
				Task.Factory.StartNew(() =>
				{
					// Playlist was changed and cache is out of sync.
					// So we start a task to update the cache.
					SyncPlaylistDataWithCache(playlist);
				});
			}
			return success;
		}

		/// <summary>
		///     Creates a new playlist.
		/// </summary>
		/// <param name="name">The name of the playlist that will be created.</param>
		/// <param name="list">
		///     A string list containing the full paths of the tracks
		///     that will be added to the playlist. If left empty an empty playlist will be created.
		/// </param>
		/// <returns>A <see cref="SuccessResponse" /> is returned.</returns>
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
				var id = db.GetLastInsertId();
				playlist.Id = id;

				// List has elements that have to be synced with the cache.
				if (list.Length > 0)
				{
					Task.Factory.StartNew(() => { SyncPlaylistDataWithCache(playlist); });
				}

				return new SuccessResponse
				{
					Success = id > 0
				};
			}
		}

		/// <summary>
		/// Moves a track in a playlist to a new position in the playlist.
		/// </summary>
		/// <param name="id">The id of the playlist.</param>
		/// <param name="from">The original position of the track in the playlist.</param>
		/// <param name="to">The new position of the track in the playlist.</param>
		/// <returns></returns>
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

			var success = _api.Playlist_MoveFiles(playlist.Path, aFrom, dIn);
			if (success)
			{
				Task.Factory.StartNew(() => { SyncPlaylistDataWithCache(playlist); });
			}
			return success;
		}

		/// <summary>
		/// Adds tracks to an existing playlist.
		/// </summary>
		/// <param name="id">The id of the playlist.</param>
		/// <param name="list">A list of the paths of the files in the filesystem.</param>
		/// <returns></returns>
		public bool PlaylistAddTracks(int id, string[] list)
		{
			var playlist = GetPlaylistById(id);
			var success = _api.Playlist_AppendFiles(playlist.Path, list);
			if (success)
			{
				Task.Factory.StartNew(() => { SyncPlaylistDataWithCache(playlist); });
			}
			return success;
		}

		/// <summary>
		/// Deletes a playlist.
		/// </summary>
		/// <param name="id">The id of the playlist to delete.</param>
		/// <returns></returns>
		public bool PlaylistDelete(int id)
		{
			var playlist = GetPlaylistById(id);
			using (var db = _cHelper.GetDbConnection())
			{
				var success = _api.Playlist_DeletePlaylist(playlist.Path);
				if (success)
				{
					playlist.DateDeleted = DateTime.UtcNow;
					db.Save(playlist);
				}
				return success;
			}
		}

		/// <summary>
		///     Retrieves a cached playlist by it's id.
		/// </summary>
		/// <param name="id">The id of a playlist</param>
		/// <returns>A <see cref="Playlist" /> object.</returns>
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