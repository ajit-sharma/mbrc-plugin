#region Dependencies

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using MusicBeePlugin.AndroidRemote.Data;
using MusicBeePlugin.AndroidRemote.Utilities;
using MusicBeePlugin.Comparers;
using MusicBeePlugin.Rest.ServiceModel.Type;
using NLog;

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
        ///     The fields (MetaData) that MusicBee Remote caches.
        /// </summary>
        private readonly Plugin.MetaDataType[] _fields =
        {
            Plugin.MetaDataType.Artist,
            Plugin.MetaDataType.AlbumArtist,
            Plugin.MetaDataType.Album,
            Plugin.MetaDataType.Genre,
            Plugin.MetaDataType.TrackTitle,
            Plugin.MetaDataType.Year,
            Plugin.MetaDataType.TrackNo,
            Plugin.MetaDataType.DiscNo
        };

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
        /// <param name="after"></param>
        /// <returns></returns>
        public PaginatedResponse<LibraryCover> GetAllCovers(int offset, int limit, long after)
        {
            using (var db = _cHelper.GetDbConnection())
            {
                var covers =
                    db.Select<LibraryCover>(
                        q => (q.DateAdded > after) || (q.DateDeleted > after) || q.DateUpdated > after);
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
                //TODO: fix the handling of cover not found 
                return null;
            }
        }

        /// <summary>
        ///     Builds the cache. Creates an association of SHA1 hashes and file paths on the local
        ///     filesystem and then updates the internal SQLite database.
        /// </summary>
        public void BuildCache()
        {
            UpdateArtistTable();
            UpdateGenreTable();
            UpdateAlbumTable();
            UpdateTrackTable();
        }

        /// <summary>
        ///     Checks for updates the <see cref="LibraryTrack" /> table.
        /// </summary>
        private void UpdateTrackTable()
        {
            string[] files = {};
            _api.Library_QueryFilesEx(string.Empty, ref files);

            using (var db = _cHelper.GetDbConnection())
            using (var trans = db.OpenTransaction())
            {
                var artists = db.Select<LibraryArtist>();
                var genres = db.Select<LibraryGenre>();
                var albums = db.Select<LibraryAlbum>();

                var cached = db.Select<LibraryTrack>(tr => tr.DateDeleted == 0).ToList();
                var deleted = db.Select<LibraryTrack>(tr => tr.DateDeleted != 0).ToList();
                var cachedPaths = cached.Select(tr => tr.Path).ToList();

                var toInsert = files.Except(cachedPaths).ToList();
                var toDelete = cachedPaths.Except(files).ToList();

                foreach (var file in toDelete)
                {
                    db.UpdateOnly(new LibraryTrack {DateDeleted = DateTime.UtcNow.ToUnixTime()},
                        o => o.Update(p => p.DateDeleted)
                            .Where(p => p.Path.Equals(file)));
                }

                foreach (var file in toInsert)
                {
                    var deletedTrack = deleted.Find(tr => tr.Path.Equals(file));
                    string[] tags = {};
                    _api.Library_GetFileTags(file, _fields, ref tags);

                    var meta = new LibraryTrackEx(tags);

                    var genre = genres.SingleOrDefault(q => q.Name == meta.Genre);
                    var artist = artists.SingleOrDefault(q => q.Name == meta.Artist);
                    var albumArtist = artists.SingleOrDefault(q => q.Name == meta.AlbumArtist);
                    var album = albums.SingleOrDefault(q => q.Name == meta.Album);

                    if (album != null && albumArtist != null)
                    {
                        album.ArtistId = albumArtist.Id;
                    }

                    var track = new LibraryTrack
                    {
                        Title = meta.Title,
                        Year = meta.Year,
                        Position = meta.Position,
                        Disc = meta.Disc,
                        GenreId = genre?.Id ?? 0,
                        AlbumArtistId = albumArtist?.Id ?? 0,
                        ArtistId = artist?.Id ?? 0,
                        AlbumId = album?.Id ?? 0,
                        Path = file
                    };

                    // Current track was detected in the list of deleted tracks.
                    // So we are going to update the existing entry.
                    if (deletedTrack != null)
                    {
                        track.Id = deletedTrack.Id;
                        track.DateUpdated = DateTime.UtcNow.ToUnixTime();
                        track.DateDeleted = 0;
                    }

                    db.Save(track);
                }

                db.UpdateAll(albums);
                trans.Commit();

                Logger.Debug("Tracks: {0} entries inserted.", toInsert.Count());
                Logger.Debug("Tracks: {0} entries deleted.", toDelete.Count());
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
        ///     Checks for changes in the <see cref="LibraryArtist" /> table.
        /// </summary>
        public void UpdateArtistTable()
        {
            using (var db = _cHelper.GetDbConnection())
            {
                var artists = GetArtistDataFromApi();
                var cachedArtists = db.Select<LibraryArtist>(la => la.DateDeleted == 0);
                var deletedArtists = db.Select<LibraryArtist>(la => la.DateDeleted != 0);

                var comparer = new LibraryArtistComparer();

                var artistsToInsert = artists.Except(cachedArtists, comparer).ToList();
                var artistsToDelete = cachedArtists.Except(artists, comparer).ToList();

                foreach (var libraryArtist in artistsToDelete)
                {
                    libraryArtist.DateDeleted = DateTime.UtcNow.ToUnixTime();
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
        ///     Checks for changes in the <see cref="LibraryGenre" /> table.
        /// </summary>
        public void UpdateGenreTable()
        {
            using (var db = _cHelper.GetDbConnection())
            {
                var genres = GetGenreDataFromApi();
                var cachedGenres = db.Select<LibraryGenre>(gen => gen.DateDeleted == 0);
                var deletedGenres = db.Select<LibraryGenre>(gen => gen.DateDeleted != 0);
                var comparer = new LibraryGenreComparer();

                var genresToInsert = genres.Except(cachedGenres, comparer).ToList();
                var genresToRemove = cachedGenres.Except(genres, comparer).ToList();

                foreach (var libraryGenre in genresToRemove)
                {
                    libraryGenre.DateDeleted = DateTime.UtcNow.ToUnixTime();
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
        ///     Checks for changes in the <see cref="LibraryAlbum" /> table.
        /// </summary>
        public void UpdateAlbumTable()
        {
            using (var db = _cHelper.GetDbConnection())
            {
                var albums = GetAlbumDataFromApi();
                var cachedAlbums = db.Select<LibraryAlbum>(gen => gen.DateDeleted == 0);
                var deletedAlbums = db.Select<LibraryAlbum>(gen => gen.DateDeleted != 0);
                var comparer = new LibraryAlbumComparer();

                var albumsToInsert = albums.Except(cachedAlbums, comparer).ToList();
                var albumsToRemove = cachedAlbums.Except(albums, comparer).ToList();

                foreach (var album in albumsToRemove)
                {
                    album.DateDeleted = DateTime.UtcNow.ToFileTimeUtc();
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
                var albums = db.Select<LibraryAlbum>();
                foreach (var album in albums)
                {
                    var trackList = db.Select<LibraryTrack>(tr => tr.AlbumId == album.Id)
                        .OrderBy(tr => tr.Position)
                        .ToList();

                    if (trackList.Count == 0)
                    {
                        continue;
                    }
                    var track = trackList[0];
                    string coverUrl = null;

                    var locations = Plugin.PictureLocations.None;
                    byte[] imageData = {};

                    _api.Library_GetArtworkEx(track.Path, 0, false, ref locations, ref coverUrl, ref imageData);

                    if (string.IsNullOrEmpty(coverUrl))
                    {
                        _api.Library_GetArtworkEx(track.Path, 0, true, ref locations, ref coverUrl, ref imageData);
                    }

                    var coverHash = !string.IsNullOrEmpty(coverUrl)
                        ? Utilities.StoreCoverToCache(coverUrl)
                        : Utilities.StoreCoverToCache(imageData);

                    if (string.IsNullOrEmpty(coverHash))
                    {
                        continue;
                    }

                    var cover = new LibraryCover
                    {
                        Hash = coverHash
                    };

                    db.Save(cover);
                    album.CoverId = (int) db.GetLastInsertId();
                }

                db.UpdateAll(albums);
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
                //TODO: fix the handling of not found.
                return null;
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
        /// <param name="after"></param>
        /// <returns></returns>
        public PaginatedResponse<LibraryTrack> GetAllTracks(int limit, int offset, long after)
        {
            using (var db = _cHelper.GetDbConnection())
            {
                var data =
                    db.Select<LibraryTrack>(
                        q => (q.DateAdded > after) || (q.DateDeleted > after) || q.DateUpdated > after);
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
        /// <param name="after"></param>
        /// <returns></returns>
        public PaginatedResponse<LibraryArtist> GetAllArtists(int limit, int offset, long after)
        {
            using (var db = _cHelper.GetDbConnection())
            {
                var data =
                    db.Select<LibraryArtist>(
                        q => (q.DateAdded > after) || (q.DateDeleted > after) || q.DateUpdated > after);
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
                    //Todo: fix the handling of not found.
                    return null;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public PaginatedResponse<LibraryGenre> GetAllGenres(int limit, int offset, long after)
        {
            using (var db = _cHelper.GetDbConnection())
            {
                var data =
                    db.Select<LibraryGenre>(
                        q => (q.DateAdded > after) || (q.DateDeleted > after) || q.DateUpdated > after);
                var paginated = new PaginatedGenreResponse();
                paginated.CreatePage(limit, offset, data);
                return paginated;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public PaginatedResponse<LibraryAlbum> GetAllAlbums(int limit, int offset, long after)
        {
            using (var db = _cHelper.GetDbConnection())
            {
                var data =
                    db.Select<LibraryAlbum>(
                        q => (q.DateAdded > after) || (q.DateDeleted > after) || q.DateUpdated > after);
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
            var list = new List<string>();
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
            var trackList = new List<string>();

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
            var tracklist = new List<string>();

            try
            {
                using (var db = _cHelper.GetDbConnection())
                {
                    var sql = "SELECT LibraryTrack.Id AS Id, " +
                              "LibraryTrack.Title AS Title, " +
                              "LibraryTrack.Path AS Path, " +
                              "LibraryTrack.Year AS Year, " +
                              "LibraryTrack.Position AS Position, " +
                              "LibraryGenre.Name AS Genre, " +
                              "artist.Name AS Artist, " +
                              "albumArtist.Name AS AlbumArtist, " +
                              "LibraryAlbum.Name AS Album " +
                              "FROM LibraryTrack " +
                              "INNER JOIN  LibraryGenre ON LibraryTrack.GenreId = LibraryGenre.Id " +
                              "LEFT OUTER JOIN  LibraryArtist artist ON LibraryTrack.ArtistId = artist.Id " +
                              "LEFT OUTER JOIN  LibraryArtist albumArtist ON LibraryTrack.AlbumArtistId = albumArtist.Id " +
                              "LEFT OUTER JOIN  LibraryAlbum ON LibraryTrack.AlbumId = LibraryAlbum.Id  " +
                              "WHERE LibraryGenre.Id = " + id + " " +
                              "ORDER BY albumArtist.Name ASC , LibraryAlbum.Name ASC ,LibraryTrack.Position ASC";
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