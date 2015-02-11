#region Dependencies

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using MusicBeePlugin.AndroidRemote.Data;
using MusicBeePlugin.AndroidRemote.Utilities;
using MusicBeePlugin.Comparers;
using MusicBeePlugin.Rest.ServiceModel.Type;
using NLog;
using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.Text;

#endregion

namespace MusicBeePlugin.Modules
{
	/// <summary>
	///     Class SyncModule.
	///     Hosts the functionality responsible for the library sync operations.
	/// </summary>
	public class LibraryModule : DataModuleBase
	{
		/// <summary>
		///     Gets the Default logger instance for the class.
		/// </summary>
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		///     Initializes a new instance of the <see cref="LibraryModule" /> class.
		/// </summary>
		public LibraryModule(Plugin.MusicBeeApiInterface api, CacheHelper cHelper)
		{
			_api = api;
			_cHelper = cHelper;
		}

		/// <summary>
		///     Checks for changes in the library and updates the cache.
		/// </summary>
		/// <param name="cachedFiles">The cached files.</param>
		/// <param name="lastSync">The last synchronization date.</param>
		public void SyncCheckForChanges(string[] cachedFiles, DateTime lastSync)
		{
			string[] newFiles = {};
			string[] deletedFiles = {};
			string[] updatedFiles = {};

			_api.Library_GetSyncDelta(cachedFiles, lastSync, Plugin.LibraryCategory.Music,
				ref newFiles, ref updatedFiles, ref deletedFiles);
		}

		/// <summary>
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="limit"></param>
		/// <returns></returns>
		public PaginatedResponse<LibraryCover> GetAllCovers(int offset, int limit)
		{
			using (var db = _cHelper.GetDbConnection())
			{
				var covers = db.Select<LibraryCover>();
				var paginated = new PaginatedCoverResponse();
				paginated.CreatePage(limit, offset, covers);
				return paginated;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="id"></param>
		/// <param name="includeImage"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public LibraryCover GetLibraryCover(int id, bool includeImage = false)
		{
			try
			{
				using (var db = _cHelper.GetDbConnection())
				{
					var cover = db.GetById<LibraryCover>(id);
					return cover;
				}
			}
			catch (Exception)
			{
				throw HttpError.NotFound("Cover resource with id {0} does not exist".Fmt(id));
			}
		}

		/// <summary>
		///     Builds the cache. Creates an association of SHA1 hashes and file paths on the local
		///     filesystem and then updates the internal SQLite database.
		/// </summary>
		public void BuildCache()
		{
			string[] files = {};
			_api.Library_QueryFilesEx(String.Empty, ref files);
			using (var db = _cHelper.GetDbConnection())
			using (var trans = db.OpenTransaction())
			{
				db.SaveAll(GetArtistDataFromApi());
				db.SaveAll(GetGenreDataFromApi());
				db.SaveAll(GetAlbumDataFromApi());
				var artists = db.Select<LibraryArtist>();
				var genres = db.Select<LibraryGenre>();
				var albums = db.Select<LibraryAlbum>();
				foreach (var file in files)
				{
					Plugin.MetaDataType[] types =
					{
						Plugin.MetaDataType.Artist,
						Plugin.MetaDataType.AlbumArtist,
						Plugin.MetaDataType.Album,
						Plugin.MetaDataType.Genre,
						Plugin.MetaDataType.TrackTitle,
						Plugin.MetaDataType.Year,
						Plugin.MetaDataType.TrackNo
					};

					var i = 0;
					string[] tags = {};
					_api.Library_GetFileTags(file, types, ref tags);

					var artist = tags[i++];
					var albumArtist = tags[i++];
					var album = tags[i++];
					var genre = tags[i++];
					var title = tags[i++];
					var year = tags[i++];
					var trackNo = tags[i];

					int iTrack;
					int.TryParse(trackNo, out iTrack);

					var oGenre = genres.SingleOrDefault(q => q.Name == genre);
					var oArtist = artists.SingleOrDefault(q => q.Name == artist);
					var oAlbumArtist = artists.SingleOrDefault(q => q.Name == albumArtist);
					var oAlbum = albums.SingleOrDefault(q => q.Name == album);

					if (oAlbum != null && oAlbumArtist != null)
					{
						oAlbum.ArtistId = oAlbumArtist.Id;
					}

					var track = new LibraryTrack
					{
						Title = title,
						Year = year,
						Position = iTrack,
						GenreId = oGenre?.Id ?? 0,
						AlbumArtistId = oAlbumArtist?.Id ?? 0,
						ArtistId = oArtist?.Id ?? 0,
						AlbumId = oAlbum?.Id ?? 0,
						Path = file
					};
					db.Save(track);
				}
				db.UpdateAll(albums);
				trans.Commit();
			}
		}

		/// <summary>
		///     Gets the artists available in the MusicBee database.
		/// </summary>
		/// <returns></returns>
		private List<LibraryArtist> GetArtistDataFromApi()
		{
			var list = new List<LibraryArtist>();
			if (_api.Library_QueryLookupTable("artist", "count", null))
			{
				list.AddRange(
					_api.Library_QueryGetLookupTableValue(null)
						.Split(new[] {"\0\0"}, StringSplitOptions.None)
						.Select(artist => new LibraryArtist(artist.Split('\0')[0])));
			}
			_api.Library_QueryLookupTable(null, null, null);
			return list;
		}

		/// <summary>
		///     Gets the genres available in the MusicBee database.
		/// </summary>
		/// <returns></returns>
		private List<LibraryGenre> GetGenreDataFromApi()
		{
			var list = new List<LibraryGenre>();
			if (_api.Library_QueryLookupTable("genre", "count", null))
			{
				list.AddRange(
					_api.Library_QueryGetLookupTableValue(null)
						.Split(new[] {"\0\0"}, StringSplitOptions.None)
						.Select(artist => new LibraryGenre(artist.Split('\0')[0])));
			}
			_api.Library_QueryLookupTable(null, null, null);
			return list;
		}

		/// <summary>
		///     Gets the Albums available in the MusicBee database.
		/// </summary>
		/// <returns></returns>
		private List<LibraryAlbum> GetAlbumDataFromApi()
		{
			var list = new List<LibraryAlbum>();
			if (_api.Library_QueryLookupTable("album", "count", null))
			{
				list.AddRange(
					_api.Library_QueryGetLookupTableValue(null)
						.Split(new[] {"\0\0"}, StringSplitOptions.None)
						.Select(artist => new LibraryAlbum
						{
							Name = artist.Split('\0')[0]
						}));
			}
			_api.Library_QueryLookupTable(null, null, null);
			return list;
		}

		/// <summary>
		/// Checks for changes in the <see cref="LibraryArtist"/> table.
		/// </summary>
		public void UpdateArtistTable()
		{
			using (var db = _cHelper.GetDbConnection())
			{
				var artists = GetArtistDataFromApi();
				var cachedArtists = db.Select<LibraryArtist>(la => la.DateDeleted == null);
				var deletedArtists = db.Select<LibraryArtist>(la => la.DateDeleted != null);

				var comparer = new LibraryArtistComparer();

				var artistsToInsert = artists.Except(cachedArtists, comparer).ToList();
				var artistsToDelete = cachedArtists.Except(artists, comparer).ToList();

				foreach (var libraryArtist in artistsToDelete)
				{
					libraryArtist.DateDeleted = DateTime.UtcNow;
				}

				if (artistsToDelete.Count > 0)
				{
					db.SaveAll(artistsToDelete);
					Logger.Debug("Artists: {0} entries deleted.", artistsToDelete.Count);
				}

				foreach (var libraryArtist in artistsToInsert)
				{
					var artist =
						deletedArtists.Find(art => art.Name
							.Equals(libraryArtist.Name,
								StringComparison.InvariantCultureIgnoreCase));
					if (artist != null)
					{
						libraryArtist.Id = artist.Id;
					}
				}

				if (artistsToInsert.Count > 0)
				{
					db.SaveAll(artistsToInsert);
					Logger.Debug("Artists: {0} entries inserted", artistsToInsert.Count);
				}
			}
		}

		/// <summary>
		/// Checks for changes in the <see cref="LibraryGenre"/> table.
		/// </summary>
		public void UpdateGenreTable()
		{
			using (var db = _cHelper.GetDbConnection())
			{
				var genres = GetGenreDataFromApi();
				var cachedGenres = db.Select<LibraryGenre>(gen => gen.DateDeleted == null);
				var deletedGenres = db.Select<LibraryGenre>(gen => gen.DateDeleted != null);
				var comparer = new LibraryGenreComparer();

				var genresToInsert = genres.Except(cachedGenres, comparer).ToList();
				var genresToRemove = cachedGenres.Except(genres, comparer).ToList();

				foreach (var libraryGenre in genresToRemove)
				{
					libraryGenre.DateDeleted = DateTime.UtcNow;
				}

				if (genresToRemove.Count > 0)
				{
					db.SaveAll(genresToRemove);
					Logger.Debug("Genres: {0} entries removed", genresToRemove.Count);
				}

				foreach (var libraryGenre in genresToInsert)
				{
					var genre =
						deletedGenres.Find(gen => gen.Name
							.Equals(libraryGenre.Name,
								StringComparison.InvariantCultureIgnoreCase));
					if (genre != null)
					{
						libraryGenre.Id = genre.Id;
					}
				}

				if (genresToInsert.Count > 0)
				{
					db.SaveAll(genresToInsert);
					Logger.Debug("Genres: {0} entries inserted", genresToInsert.Count);
				}

			}
			
		}

		/// <summary>
		/// Checks for changes in the <see cref="LibraryAlbum"/> table.
		/// </summary>
		public void UpdateAlbumTable()
		{
			using (var db = _cHelper.GetDbConnection())
			{
				var albums = GetAlbumDataFromApi();
				var cachedAlbums = db.Select<LibraryAlbum>(gen => gen.DateDeleted == null);
				var deletedAlbums = db.Select<LibraryAlbum>(gen => gen.DateDeleted != null);
				var comparer = new LibraryAlbumComparer();

				var albumsToInsert = albums.Except(cachedAlbums, comparer).ToList();
				var albumsToRemove = cachedAlbums.Except(albums, comparer).ToList();

				foreach (var album in albumsToRemove)
				{
					album.DateDeleted = DateTime.UtcNow;
				}

				if (albumsToRemove.Count > 0)
				{
					db.SaveAll(albumsToRemove);
					Logger.Debug("Albums: {0} entries removed", albumsToRemove.Count);
				}

				foreach (var albumEntry in albumsToInsert)
				{
					var album =
						deletedAlbums.Find(gen => gen.Name
							.Equals(albumEntry.Name,
								StringComparison.InvariantCultureIgnoreCase));
					if (album != null)
					{
						albumEntry.Id = album.Id;
					}
				}

				if (albumsToInsert.Count > 0)
				{
					db.SaveAll(albumsToInsert);
					Logger.Debug("Albums: {0} entries inserted", albumsToInsert.Count);
				}

			}

		}

		/// <summary>
		///     Builds the cover cache per album.
		///     This method is faster because it calls the GetArtworkUrl method for the first track of each album,
		///     however it might miss a number of covers;
		/// </summary>
		public void BuildCoverCachePerAlbum()
		{
			using (var db = _cHelper.GetDbConnection())
			using (var trans = db.OpenTransaction())
			{
				var allTrack = db.Select<LibraryTrack>();
				var map = new Dictionary<string, LibraryAlbum>();
				var albums = db.Select<LibraryAlbum>();

				foreach (var lTrack in allTrack)
				{
					var path = lTrack.Path;
					var id = _api.Library_GetFileTag(path, Plugin.MetaDataType.AlbumId);
					LibraryAlbum ab;
					if (!map.TryGetValue(id, out ab))
					{
						ab = albums.SingleOrDefault(q => q.Id == lTrack.AlbumId) ?? new LibraryAlbum();
						ab.AlbumId = id;
						map.Add(id, ab);
					}
					var trackId = _api.Library_GetFileTag(path, Plugin.MetaDataType.TrackNo);
					var track = new LibraryTrack
					{
						Path = path,
						Position = !string.IsNullOrEmpty(trackId) ? int.Parse(trackId, NumberStyles.Any) : 0
					};
					ab.TrackList.Add(track);
				}

				var list = new List<LibraryAlbum>(map.Values);

				foreach (var albumEntry in list)
				{
					albumEntry.TrackList.Sort();
					var path = albumEntry.TrackList[0].Path;
					String coverUrl = null;

					var locations = Plugin.PictureLocations.None;
					byte[] imageData = {};

					_api.Library_GetArtworkEx(path, 0, false, ref locations, ref coverUrl, ref imageData);

					if (String.IsNullOrEmpty(coverUrl))
					{
						_api.Library_GetArtworkEx(path, 0, true, ref locations, ref coverUrl, ref imageData);
					}

					var coverHash = !String.IsNullOrEmpty(coverUrl)
						? Utilities.StoreCoverToCache(coverUrl)
						: Utilities.StoreCoverToCache(imageData);

					if (String.IsNullOrEmpty(coverHash))
					{
						continue;
					}

					var cover = new LibraryCover
					{
						Hash = coverHash
					};

					db.Save(cover);
					albumEntry.CoverId = (int) db.GetLastInsertId();
				}

				db.UpdateAll(list);
				trans.Commit();
			}
		}

		/// <summary>
		///     Builds the artist cover cache.
		///     Method is really slow, due to multiple threads being called.
		///     Should be better called on a low priority thread.
		/// </summary>
		public void BuildArtistCoverCache()
		{
			var artistList = new List<LibraryArtist>();
			if (_api.Library_QueryLookupTable("artist", "count", ""))
			{
				artistList.AddRange(
					_api.Library_QueryGetLookupTableValue(null)
						.Split(new[] {"\0\0"}, StringSplitOptions.None)
						.Select(entry => entry.Split('\0'))
						.Select(artistInfo => new LibraryArtist(artistInfo[0])));
			}

			_api.Library_QueryLookupTable(null, null, null);
			foreach (var entry in artistList)
			{
				string[] urls = {};
				var artist = entry.Name;
				_api.Library_GetArtistPictureUrls(artist, true, ref urls);
				if (urls.Length <= 0) continue;
				var hash = Utilities.CacheArtistImage(urls[0], artist);
				entry.ImageUrl = hash;
			}
		}

		/// <summary>
		///     Retrieves a <see cref="LibraryTrack" /> from the cache using it's id.
		/// </summary>
		/// <param name="id">The id of the entry in the database.</param>
		/// <returns></returns>
		/// <exception cref="Exception">
		///     <see cref="HttpError.NotFound" />
		/// </exception>
		public LibraryTrack GetTrackById(int id)
		{
			try
			{
				using (var db = _cHelper.GetDbConnection())
				{
					return db.GetByIdOrDefault<LibraryTrack>(id);
				}
			}
			catch (Exception)
			{
				throw HttpError.NotFound("Track resource with id {0} does not exist".Fmt(id));
			}
		}

		/// <summary>
		///     This method checks the state of the cache and is responsible for either
		///     building the cache when empty of updating on start.
		/// </summary>
		public bool IsCacheEmpty()
		{
			var cached = GetCachedEntitiesCount<LibraryTrack>();
			return cached == 0;
		}

		/// <summary>
		///     Retrieves A number of tracks from the database and returns a
		///     Paginated response.
		/// </summary>
		/// <param name="limit">
		///     The number of results in the response. If the
		///     limit equals 0 then all the data are returned.
		/// </param>
		/// <param name="offset">The index of the first result.</param>
		/// <returns></returns>
		public PaginatedResponse<LibraryTrack> GetAllTracks(int limit, int offset)
		{
			using (var db = _cHelper.GetDbConnection())
			{
				var data = db.Select<LibraryTrack>(q => q.OrderBy(x => x.Id));
				var paginatedResult = new PaginatedTrackResponse();
				paginatedResult.CreatePage(limit, offset, data);
				return paginatedResult;
			}
		}

		/// <summary>
		///     Retrieves a number of <see cref="LibraryArtist" /> results (a page) from the cache.
		/// </summary>
		/// <param name="limit">The number of results in the page.</param>
		/// <param name="offset">The first position in the result set.</param>
		/// <returns></returns>
		public PaginatedResponse<LibraryArtist> GetAllArtists(int limit, int offset)
		{
			using (var db = _cHelper.GetDbConnection())
			{
				var data = db.Select<LibraryArtist>(q => q.OrderBy(x => x.Id));
				var paginated = new PaginatedArtistResponse();
				paginated.CreatePage(limit, offset, data);
				return paginated;
			}
		}

		/// <summary>
		///     Gets an artist from the cache.
		/// </summary>
		/// <param name="id">The id of the artist.</param>
		/// <returns>The cached <see cref="LibraryArtist" /> for the provided id.</returns>
		/// <exception cref="Exception">
		///     <see cref="HttpError.NotFound" />
		/// </exception>
		public LibraryArtist GetArtistById(int id)
		{
			using (var db = _cHelper.GetDbConnection())
			{
				try
				{
					return db.GetById<LibraryArtist>(id);
				}
				catch
				{
					throw HttpError.NotFound("Artist resource with id {0} does not exist".Fmt(id));
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="limit"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		public PaginatedResponse<LibraryGenre> GetAllGenres(int limit, int offset)
		{
			using (var db = _cHelper.GetDbConnection())
			{
				var data = db.Select<LibraryGenre>(q => q.OrderBy(x => x.Id));
				var paginated = new PaginatedGenreResponse();
				paginated.CreatePage(limit, offset, data);
				return paginated;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="limit"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		public PaginatedResponse<LibraryAlbum> GetAllAlbums(int limit, int offset)
		{
			using (var db = _cHelper.GetDbConnection())
			{
				var data = db.Select<LibraryAlbum>(q => q.OrderBy(x => x.Id));
				var paginated = new PaginatedAlbumResponse();
				paginated.CreatePage(limit, offset, data);
				return paginated;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Stream GetCoverData(int id)
		{
			var cover = GetLibraryCover(id);
			return Utilities.GetCoverStreamFromCache(cover.Hash);
		}

		/// <summary>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public int GetCachedEntitiesCount<T>()
		{
			var total = 0;
			try
			{
				using (var db = _cHelper.GetDbConnection())
				{
					total = db.Select<T>().Count;
				}
			}
			catch (Exception e)
			{
				Logger.Debug(e);
			}
			return total;
		}

		/// <summary>
		///     Given a long id of a an artist in the database
		///     it will return a String array of the paths of the Artist's
		///     album tracks ordered by album name and track position
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string[] GetArtistTracksById(long id)
		{
			var trackList = new List<string>();
			try
			{
				using (var db = _cHelper.GetDbConnection())
				{
					var albumList = db.Select<LibraryAlbum>(q => q.ArtistId == id)
						.OrderBy(x => x.Name).ToList();
					foreach (var albumTrackList in albumList
						.Select(album => GetTrackListByAlbumId(db, album.Id)))
					{
						trackList.AddRange(albumTrackList.Select(t => t.Path).ToList());
					}
				}
			}
			catch (Exception e)
			{
				Logger.Debug(e);
			}

			return trackList.ToArray();
		}

		/// <summary>
		///     Given a database connection and an album id it will return a list of Tracks
		///     ordered by position.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		private static List<LibraryTrack> GetTrackListByAlbumId(IDbConnection db, long id)
		{
			return db.Select<LibraryTrack>(q => q.AlbumId == id)
				.OrderBy(x => x.Position)
				.ToList();
		}

		/// <summary>
		///     Given an id in the database it will retrieve the path of the track.
		///     It returns an array instead of a single String to be in consistency
		///     with the other group of methods. <see cref="GetAlbumTracksById" /> etc.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string[] GetTrackPathById(long id)
		{
			var list = new List<String>();
			try
			{
				using (var db = _cHelper.GetDbConnection())
				{
					list = db.Select<LibraryTrack>(q => q.Id == id).Select(t => t.Path).ToList();
				}
			}
			catch (Exception e)
			{
				Logger.Debug(e);
			}

			return list.ToArray();
		}

		/// <summary>
		///     Given an album track it will return the paths of the tracks
		///     included in the album.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string[] GetAlbumTracksById(long id)
		{
			var trackList = new List<String>();

			try
			{
				using (var db = _cHelper.GetDbConnection())
				{
					trackList.AddRange(GetTrackListByAlbumId(db, id).Select(t => t.Path));
				}
			}
			catch (Exception e)
			{
				Logger.Debug(e);
			}
			return trackList.ToArray();
		}

		/// <summary>
		///     Given a genre <paramref name="id" /> in the database it will return a
		///     <see cref="String" />array of the paths in the file system
		///     representing the tracks, ordered by artist, album  and position in
		///     the album.
		/// </summary>
		/// <param name="id">The id of the genre</param>
		/// <returns></returns>
		public string[] GetGenreTracksById(long id)
		{
			var tracklist = new List<String>();

			try
			{
				using (var db = _cHelper.GetDbConnection())
				{
					var join = new JoinSqlBuilder<LibraryTrackEx, LibraryTrack>();
					join = join.Join<LibraryTrack, LibraryGenre>(track => track.GenreId,
						genre => genre.Id,
						tr => new {tr.Id, tr.Title, tr.Path, tr.Year, tr.Position}, x => new {Genre = x.Name})
						.LeftJoin<LibraryTrack, LibraryArtist>(t => t.ArtistId, a => a.Id, null, x => new {Artist = x.Name})
						.LeftJoin<LibraryTrack, LibraryArtist>(t => t.AlbumArtistId, a => a.Id, null, x => new {AlbumArtist = x.Name})
						.LeftJoin<LibraryTrack, LibraryAlbum>(t => t.AlbumId, a => a.Id, null, x => new {Album = x.Name})
						.OrderBy<LibraryArtist>(artist => artist.Name)
						.OrderBy<LibraryAlbum>(album => album.Name)
						.OrderBy<LibraryTrack>(track => track.Position);
					var sql = join.ToSql();
					var result = db.Query<LibraryTrackEx>(sql);
					tracklist.AddRange(result.Select(track => track.Path));
				}
			}
			catch (Exception e)
			{
				Logger.Debug(e);
			}

			return tracklist.ToArray();
		}
	}
}