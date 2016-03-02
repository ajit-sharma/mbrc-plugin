namespace MusicBeeRemoteCore.Modules
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;

    using MusicBeeRemoteCore.AndroidRemote.Enumerations;
    using MusicBeeRemoteCore.AndroidRemote.Utilities;
    using MusicBeeRemoteCore.Comparers;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Extensions;
    using MusicBeeRemoteData.Repository.Interfaces;

    using NLog;

    /// <summary>
    ///     Class SyncModule.
    ///     Hosts the functionality responsible for the library sync operations.
    /// </summary>
    public class LibraryModule
    {
        /// <summary>
        ///     Gets the Default logger instance for the class.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IAlbumRepository albumRepository;

        private readonly ILibraryApiAdapter api;

        private readonly IArtistRepository artistRepository;

        private readonly ICoverRepository coverRepository;

        private readonly IGenreRepository genreRepository;

        private readonly ITrackRepository trackRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LibraryModule" /> class.
        /// </summary>
        public LibraryModule(
            ILibraryApiAdapter api, 
            IGenreRepository genreRepository, 
            IArtistRepository artistRepository, 
            IAlbumRepository albumRepository, 
            ITrackRepository trackRepository, 
            ICoverRepository coverRepository)
        {
            this.albumRepository = albumRepository;
            this.trackRepository = trackRepository;
            this.coverRepository = coverRepository;
            this.genreRepository = genreRepository;
            this.artistRepository = artistRepository;
            this.api = api;
        }

        /// <summary>
        ///     Builds the artist cover cache.
        ///     Method is really slow, due to multiple threads being called.
        ///     Should be better called on a low priority thread.
        /// </summary>
        public void BuildArtistCoverCache()
        {
            var artists = this.api.GetArtistList();
            foreach (var artist in artists)
            {
                var name = artist.Name;
                var imageUrl = this.api.GetArtistUrl(name);

                if (string.IsNullOrEmpty(imageUrl))
                {
                    continue;
                }

                var hash = Utilities.CacheArtistImage(imageUrl, name);
                artist.ImageUrl = hash;

                // Todo: update the artist entry
            }
        }

        /// <summary>
        ///     Builds the cache. Creates an association of SHA1 hashes and file paths on the local
        ///     filesystem and then updates the internal SQLite database.
        /// </summary>
        public void BuildCache()
        {
            this.UpdateArtistTable();
            this.UpdateGenreTable();
            this.UpdateAlbumTable();
            this.UpdateTrackTable();
        }

        /// <summary>
        ///     Builds the cover cache per album.
        ///     This method is faster because it calls the GetArtworkUrl method for the first track of each album,
        ///     however it might miss a number of covers;
        /// </summary>
        public void BuildCoverCachePerAlbum()
        {
            var albums = this.albumRepository.GetAll();

            foreach (var album in albums)
            {
                var trackList = this.trackRepository.GetTracksByAlbumId(album.Id);

                if (!trackList.Any())
                {
                    continue;
                }

                var track = trackList.First();

                var coverUrl = this.api.GetCoverUrl(track.Path);
                string hash;

                if (string.IsNullOrEmpty(coverUrl))
                {
                    var data = this.api.GetCoverData(track.Path);
                    hash = Utilities.StoreCoverToCache(data);
                }
                else
                {
                    hash = Utilities.StoreCoverToCache(coverUrl);
                }

                if (string.IsNullOrEmpty(hash))
                {
                    continue;
                }

                var cover = new LibraryCover { Hash = hash };
                album.CoverId = this.coverRepository.Save(cover);
            }

            this.albumRepository.Save(albums);
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
            var tracks = this.trackRepository.GetTracksByAlbumId(id);
            trackList.AddRange(tracks.Select(track => track.Path));
            return trackList.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public PaginatedResponse<LibraryAlbum> GetAllAlbums(int limit, int offset, long after)
        {
            var albums = this.albumRepository.GetUpdatedPage(offset, limit, after);
            var total = this.albumRepository.GetCount();
            var paginated = new PaginatedAlbumResponse
                                {
                                    Total = total, 
                                    Offset = offset, 
                                    Limit = limit, 
                                    Data = albums.ToList()
                                };

            return paginated;
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
            var artists = this.artistRepository.GetUpdatedPage(offset, limit, after);
            var total = this.artistRepository.GetCount();
            var paginated = new PaginatedArtistResponse
                                {
                                    Total = total, 
                                    Offset = offset, 
                                    Limit = limit, 
                                    Data = artists.ToList()
                                };

            return paginated;
        }

        /// <summary>
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public PaginatedResponse<LibraryCover> GetAllCovers(int offset, int limit, long after)
        {
            var updatedCovers = this.coverRepository.GetUpdatedPage(offset, limit, after);
            var total = this.coverRepository.GetCount();
            var paginatedResponse = new PaginatedCoverResponse
                                        {
                                            Total = total, 
                                            Limit = limit, 
                                            Offset = offset, 
                                            Data = updatedCovers.ToList()
                                        };

            return paginatedResponse;
        }

        /// <summary>
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public PaginatedResponse<LibraryGenre> GetAllGenres(int limit, int offset, long after)
        {
            var genres = this.genreRepository.GetUpdatedPage(limit, offset, after);
            var total = this.genreRepository.GetCount();
            var paginated = new PaginatedGenreResponse
                                {
                                    Total = total, 
                                    Offset = offset, 
                                    Limit = limit, 
                                    Data = genres.ToList()
                                };

            return paginated;
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
            var tracks = this.trackRepository.GetUpdatedPage(offset, limit, after);
            var total = this.trackRepository.GetCount();
            var paginated = new PaginatedTrackResponse
                                {
                                    Total = total, 
                                    Offset = offset, 
                                    Limit = limit, 
                                    Data = tracks.ToList()
                                };

            return paginated;
        }

        /// <summary>
        ///     Gets an artist from the cache.
        /// </summary>
        /// <param name="id">The id of the artist.</param>
        /// <returns>The cached <see cref="LibraryArtist" /> for the provided id.</returns>
        public LibraryArtist GetArtistById(int id)
        {
            return this.artistRepository.GetById(id);
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

            // try
            // {
            // using (var db = _cHelper.GetDbConnection())
            // {
            // var albumList = db.Select<LibraryAlbum>(q => q.ArtistId == id)
            // .OrderBy(x => x.Name).ToList();
            // var albums = this.albumRepository.getAlbumsByArtist(id);
            // foreach (
            // var albumTrackList in
            // albumList.Select(album => this.trackRepository.GetTracksByAlbumId(album.Id))) ;
            // {
            // trackList.AddRange(albumTrackList.Select(t => t.Path).ToList());
            // }
            // }
            // }
            // catch (Exception e)
            // {
            // Logger.Debug(e);
            // }
            return trackList.ToArray();
        }

        public int GetCachedCoverCount()
        {
            return this.coverRepository.GetCount();
        }

        public int GetCachedTrackCount()
        {
            return this.trackRepository.GetCount();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Stream GetCoverData(int id)
        {
            var cover = this.GetLibraryCover(id);
            return Utilities.GetCoverStreamFromCache(cover.Hash);
        }

        /// <summary>
        ///     Given a genre <paramref name="id" /> in the database it will return a
        ///     <see cref="string" />array of the paths in the file system
        ///     representing the tracks, ordered by artist, album  and position in
        ///     the album.
        /// </summary>
        /// <param name="id">The id of the genre</param>
        /// <returns></returns>
        public string[] GetGenreTracksById(long id)
        {
            var tracklist = new List<string>();

            // try
            // {
            // using (var db = _cHelper.GetDbConnection())
            // {
            // var sql = "SELECT LibraryTrack.Id AS Id, " + "LibraryTrack.Title AS Title, "
            // + "LibraryTrack.Path AS Path, " + "LibraryTrack.Year AS Year, "
            // + "LibraryTrack.Position AS Position, " + "LibraryGenre.Name AS Genre, "
            // + "artist.Name AS Artist, " + "albumArtist.Name AS AlbumArtist, "
            // + "LibraryAlbum.Name AS Album " + "FROM LibraryTrack "
            // + "INNER JOIN  LibraryGenre ON LibraryTrack.GenreId = LibraryGenre.Id "
            // + "LEFT OUTER JOIN  LibraryArtist artist ON LibraryTrack.ArtistId = artist.Id "
            // + "LEFT OUTER JOIN  LibraryArtist albumArtist ON LibraryTrack.AlbumArtistId = albumArtist.Id "
            // + "LEFT OUTER JOIN  LibraryAlbum ON LibraryTrack.AlbumId = LibraryAlbum.Id  "
            // + "WHERE LibraryGenre.Id = " + id + " "
            // + "ORDER BY albumArtist.Name ASC , LibraryAlbum.Name ASC ,LibraryTrack.Position ASC";
            // var result = db.Query<LibraryTrackEx>(sql);
            // tracklist.AddRange(result.Select(track => track.Path));
            // }
            // }
            // catch (Exception e)
            // {
            // Logger.Debug(e);
            // }
            return tracklist.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeImage"></param>
        /// <returns></returns>       
        public LibraryCover GetLibraryCover(int id, bool includeImage = false)
        {
            return this.coverRepository.GetById(id);
        }

        /// <summary>
        ///     Retrieves a <see cref="LibraryTrack" /> from the cache using it's id.
        /// </summary>
        /// <param name="id">The id of the entry in the database.</param>
        /// <returns></returns>
        public LibraryTrack GetTrackById(int id)
        {
            return this.trackRepository.GetById(id);
        }

        /// <summary>
        /// Gets the track list based on the meta tag type provided and the id of the item.
        /// </summary>
        /// <param name="tag">The tag defining the type of the metadata, genre, artist etc <see cref="MetaTag"/></param>
        /// <param name="id">The id of the item in the database</param>
        /// <returns>A list of paths in the file system belonging to tracks matching the supplied parameters.</returns>
        public string[] GetTracklist(MetaTag tag, long id)
        {
            string[] tracklist;
            switch (tag)
            {
                case MetaTag.artist:
                    tracklist = this.GetArtistTracksById(id);
                    break;
                case MetaTag.album:
                    tracklist = this.GetAlbumTracksById(id);
                    break;
                case MetaTag.genre:
                    tracklist = this.GetGenreTracksById(id);
                    break;
                case MetaTag.track:
                    tracklist = this.GetTrackPathById(id);
                    break;
                default:
                    tracklist = new string[] { };
                    break;
            }

            return tracklist;
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
            var track = this.trackRepository.GetById(id);
            if (track != null)
            {
                list.Add(track.Path);
            }

            return list.ToArray();
        }

        /// <summary>
        ///     This method checks the state of the cache and is responsible for either
        ///     building the cache when empty of updating on start.
        /// </summary>
        public bool IsCacheEmpty()
        {
            return this.trackRepository.GetCount() == 0;
        }

        /// <summary>
        ///     Checks for changes in the library and updates the cache.
        /// </summary>
        /// <param name="cachedFiles">The cached files.</param>
        /// <param name="lastSync">The last synchronization date.</param>
        public void SyncCheckForChanges(string[] cachedFiles, DateTime lastSync)
        {
            this.api.GetSyncDelta(cachedFiles, lastSync);
        }

        /// <summary>
        ///     Checks for changes in the <see cref="LibraryAlbum" /> table.
        /// </summary>
        public void UpdateAlbumTable()
        {
            var albums = this.api.GetAlbumList();
            var cachedAlbums = this.albumRepository.GetCached();
            var deletedAlbums = this.albumRepository.GetDeleted();
            var comparer = new LibraryAlbumComparer();

            var albumsToInsert = albums.Except(cachedAlbums, comparer).ToList();
            var albumsToRemove = cachedAlbums.Except(albums, comparer).ToList();

            foreach (var album in albumsToRemove)
            {
                album.DateDeleted = DateTime.UtcNow.ToFileTimeUtc();
            }

            if (albumsToRemove.Count > 0)
            {
                this.albumRepository.Delete(albumsToRemove);
                Logger.Debug("Albums: {0} entries removed", albumsToRemove.Count);
            }

          if (deletedAlbums.Count != 0) {

            foreach (var albumEntry in albumsToInsert)
            {
                var album =
                    deletedAlbums.First(
                        gen => gen.Name.Equals(albumEntry.Name, StringComparison.InvariantCultureIgnoreCase));
                if (album != null)
                {
                    albumEntry.Id = album.Id;
                }
            }
            }

            if (albumsToInsert.Count > 0)
            {
                this.albumRepository.Save(albumsToInsert);
                Logger.Debug("Albums: {0} entries inserted", albumsToInsert.Count);
            }
        }

        /// <summary>
        ///     Checks for changes in the <see cref="LibraryArtist" /> table.
        /// </summary>
        public void UpdateArtistTable()
        {
            var artists = this.api.GetArtistList();
            var cachedArtists = this.artistRepository.GetCached();
            var deletedArtists = this.artistRepository.GetDeleted();

            var comparer = new LibraryArtistComparer();

            var artistsToInsert = artists.Except(cachedArtists, comparer).ToList();
            var artistsToDelete = cachedArtists.Except(artists, comparer).ToList();

            foreach (var libraryArtist in artistsToDelete)
            {
                libraryArtist.DateDeleted = DateTime.UtcNow.ToUnixTime();
            }

            if (artistsToDelete.Count > 0)
            {
                this.artistRepository.Delete(artistsToDelete);
                Logger.Debug("Artists: {0} entries deleted.", artistsToDelete.Count);
            }

            if (deletedArtists.Count > 0)
            {
                foreach (var libraryArtist in artistsToInsert)
                {
                    var artist =
                        deletedArtists.First(
                            art => art.Name.Equals(libraryArtist.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (artist != null)
                    {
                        libraryArtist.Id = artist.Id;
                    }
                }
            }
            

            if (artistsToInsert.Count > 0)
            {
                this.artistRepository.Save(artistsToInsert);
                Logger.Debug("Artists: {0} entries inserted", artistsToInsert.Count);
            }
        }

        /// <summary>
        ///     Checks for changes in the <see cref="LibraryGenre" /> table.
        /// </summary>
        public void UpdateGenreTable()
        {
            var genres = this.api.GetGenreList();
            var cachedGenres = this.genreRepository.GetCached();
            var deletedGenres = this.genreRepository.GetDeleted();
            var comparer = new LibraryGenreComparer();

            var genresToInsert = genres.Except(cachedGenres, comparer).ToList();
            var genresToRemove = cachedGenres.Except(genres, comparer).ToList();

            foreach (var libraryGenre in genresToRemove)
            {
                libraryGenre.DateDeleted = DateTime.UtcNow.ToUnixTime();
            }

            if (genresToRemove.Count > 0)
            {
                this.genreRepository.Delete(genresToRemove);
                Logger.Debug("Genres: {0} entries removed", genresToRemove.Count);
            }

            if (deletedGenres.Count != 0)
            {
                foreach (var libraryGenre in genresToInsert)
                {
                    var genre =
                        deletedGenres.First(
                            gen => gen.Name.Equals(libraryGenre.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (genre != null)
                    {
                        libraryGenre.Id = genre.Id;
                    }
                }
            }
            
            if (genresToInsert.Count <= 0)
            {
                return;
            }

            this.genreRepository.Save(genresToInsert);
            Logger.Debug("Genres: {0} entries inserted", genresToInsert.Count);
        }

        /// <summary>
        ///     Checks for updates the <see cref="LibraryTrack" /> table.
        /// </summary>
        private void UpdateTrackTable()
        {
            var files = this.api.GetLibraryFiles();

            var artists = this.artistRepository.GetAll();
            var genres = this.genreRepository.GetAll();
            var albums = this.albumRepository.GetAll();

            var cached = this.trackRepository.GetCached();
            var deleted = this.trackRepository.GetDeleted();
            var cachedPaths = cached.Select(tr => tr.Path).ToList();

            var toInsert = files.Except(cachedPaths).ToList();
            var toDelete = cachedPaths.Except(files).ToList();

            cached.ToObservable()
                .Where(s => toDelete.Contains(s.Path))
                .ToList()
                .DefaultIfEmpty(new List<LibraryTrack>())
                .Subscribe(
                    list =>
                        {
                            if (list.Count == 0)
                            {
                                return;
                            }
                            this.trackRepository.SoftDelete(list);
                        }, exception => Logger.Debug(exception));

            Logger.Debug($"data to insert: {toInsert.Count}, data to delete: {toDelete.Count} ");

            foreach (var file in toInsert)
            {
                var deletedTrack = deleted.FirstOrDefault(tr => tr.Path.Equals(file));

                var meta = this.api.GetTags(file);

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

                this.trackRepository.Save(track);
            }

            this.albumRepository.Save(albums);

            Logger.Debug("Tracks: {0} entries inserted.", toInsert.Count());
            Logger.Debug("Tracks: {0} entries deleted.", toDelete.Count());
        }
    }
}