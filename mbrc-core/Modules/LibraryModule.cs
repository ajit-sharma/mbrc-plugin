using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using MusicBeeRemoteCore.AndroidRemote.Utilities;
using MusicBeeRemoteCore.Comparers;
using MusicBeeRemoteCore.Rest.ServiceModel.Type;
using MusicBeeRemoteData.Entities;
using MusicBeeRemoteData.Extensions;
using MusicBeeRemoteData.Repository.Interfaces;
using NLog;

namespace MusicBeeRemoteCore.Modules
{
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
            var artists = api.GetArtistList();
            foreach (var artist in artists)
            {
                var name = artist.Name;
                var imageUrl = api.GetArtistUrl(name);

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
            UpdateArtistTable();
            UpdateGenreTable();
            UpdateAlbumTable();
            UpdateTrackTable();
        }

        /// <summary>
        ///     Builds the cover cache per album.
        ///     This method is faster because it calls the GetArtworkUrl method for the first track of each album,
        ///     however it might miss a number of covers;
        /// </summary>
        public void BuildCoverCachePerAlbum()
        {
            var albums = albumRepository.GetAll();
            var cachedCovers = coverRepository.GetAll();
            Logger.Debug($"Start cover processing, currently cached {cachedCovers.Count} entrie");

            var updatedAlbums = new List<LibraryAlbum>();

            var albumCovers = new Dictionary<LibraryAlbum, LibraryCover>();

            foreach (var album in albums)
            {
                var firstAlbumTrack = trackRepository.GetFirstAlbumTrackPathById(album.Id);

                if (firstAlbumTrack == null)
                {
                    continue;
                }

                var coverUrl = api.GetCoverUrl(firstAlbumTrack);
                string hash;

                if (string.IsNullOrEmpty(coverUrl))
                {
                    var data = api.GetCoverData(firstAlbumTrack);
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

                var cached = cachedCovers.Select(libraryCover => libraryCover.Hash = hash).FirstOrDefault();

                if (cached != null)
                {
                    Logger.Debug($"Cached entry detected for {hash}");
                    continue;
                }

                var cover = new LibraryCover {Hash = hash};
                albumCovers.Add(album, cover);
            }

            var libraryCovers = albumCovers.Values.ToList();
            coverRepository.Save(libraryCovers);

            foreach (var pair in albumCovers)
            {
                pair.Key.CoverId = pair.Value.Id;
            }

            var updated = albumRepository.Save(albumCovers.Keys.ToList());
            Logger.Debug($"Albums updated {updated}, covers updated {updated}");
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
            var tracks = trackRepository.GetTracksByAlbumId(id);
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
            var albums = albumRepository.GetUpdatedPage(offset, limit, after);
            var total = albumRepository.GetCount();
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
            var artists = artistRepository.GetUpdatedPage(offset, limit, after);
            var total = artistRepository.GetCount();
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
            var updatedCovers = coverRepository.GetUpdatedPage(offset, limit, after);
            var total = coverRepository.GetCount();
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
            var genres = genreRepository.GetUpdatedPage(offset, limit, after);
            var total = genreRepository.GetCount();
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
            var tracks = trackRepository.GetUpdatedPage(offset, limit, after);
            var total = trackRepository.GetCount();
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
            return artistRepository.GetById(id);
        }

        public int GetCachedCoverCount()
        {
            return coverRepository.GetCount();
        }

        public int GetCachedTrackCount()
        {
            return trackRepository.GetCount();
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
        /// <param name="id"></param>
        /// <param name="includeImage"></param>
        /// <returns></returns>
        public LibraryCover GetLibraryCover(int id, bool includeImage = false)
        {
            return coverRepository.GetById(id);
        }

        /// <summary>
        ///     Retrieves a <see cref="LibraryTrack" /> from the cache using it's id.
        /// </summary>
        /// <param name="id">The id of the entry in the database.</param>
        /// <returns></returns>
        public LibraryTrack GetTrackById(int id)
        {
            return trackRepository.GetById(id);
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
            var track = trackRepository.GetById(id);
            if (track != null)
            {
                list.Add(track.Path);
            }

            return list.ToArray();
        }

        /// <summary>
        ///     Checks for changes in the library and updates the cache.
        /// </summary>
        /// <param name="cachedFiles">The cached files.</param>
        /// <param name="lastSync">The last synchronization date.</param>
        public void SyncCheckForChanges(string[] cachedFiles, DateTime lastSync)
        {
            api.GetSyncDelta(cachedFiles, lastSync);
        }

        /// <summary>
        ///     Checks for changes in the <see cref="LibraryAlbum" /> table.
        /// </summary>
        public void UpdateAlbumTable()
        {
            var albums = api.GetAlbumList();
            var cachedAlbums = albumRepository.GetCached();
            var deletedAlbums = albumRepository.GetDeleted();
            var comparer = new LibraryAlbumComparer();

            var albumsToInsert = albums.Except(cachedAlbums, comparer).ToList();
            var albumsToRemove = cachedAlbums.Except(albums, comparer).ToList();

            foreach (var album in albumsToRemove)
            {
                album.DateDeleted = DateTime.UtcNow.ToFileTimeUtc();
            }

            if (albumsToRemove.Count > 0)
            {
                albumRepository.Delete(albumsToRemove);
                Logger.Debug($"Albums: {albumsToRemove.Count} entries removed");
            }

            if (deletedAlbums.Count != 0)
            {
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

            if (albumsToInsert.Count <= 0) return;

            albumRepository.Save(albumsToInsert);
            Logger.Debug($"Albums: {albumsToInsert.Count} entries inserted");
        }

        /// <summary>
        ///     Checks for changes in the <see cref="LibraryArtist" /> table.
        /// </summary>
        public void UpdateArtistTable()
        {
            var artists = api.GetArtistList();
            var cachedArtists = artistRepository.GetCached();
            var deletedArtists = artistRepository.GetDeleted();

            var comparer = new LibraryArtistComparer();

            var artistsToInsert = artists.Except(cachedArtists, comparer).ToList();
            var artistsToDelete = cachedArtists.Except(artists, comparer).ToList();

            foreach (var libraryArtist in artistsToDelete)
            {
                libraryArtist.DateDeleted = DateTime.UtcNow.ToUnixTime();
            }

            if (artistsToDelete.Count > 0)
            {
                artistRepository.Delete(artistsToDelete);
                Logger.Debug($"Artists: {artistsToDelete.Count} entries deleted.");
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


            if (artistsToInsert.Count <= 0) return;

            artistRepository.Save(artistsToInsert);
            Logger.Debug($"Artists: {artistsToInsert.Count} entries inserted");
        }

        /// <summary>
        ///     Checks for changes in the <see cref="LibraryGenre" /> table.
        /// </summary>
        public void UpdateGenreTable()
        {
            var genres = api.GetGenreList();
            var cachedGenres = genreRepository.GetCached();
            var deletedGenres = genreRepository.GetDeleted();
            var comparer = new LibraryGenreComparer();

            var genresToInsert = genres.Except(cachedGenres, comparer).ToList();
            var genresToRemove = cachedGenres.Except(genres, comparer).ToList();

            foreach (var libraryGenre in genresToRemove)
            {
                libraryGenre.DateDeleted = DateTime.UtcNow.ToUnixTime();
            }

            if (genresToRemove.Count > 0)
            {
                genreRepository.Delete(genresToRemove);
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

            genreRepository.Save(genresToInsert);
            Logger.Debug("Genres: {0} entries inserted", genresToInsert.Count);
        }

        /// <summary>
        ///     Checks for updates the <see cref="LibraryTrack" /> table.
        /// </summary>
        private void UpdateTrackTable()
        {
            var files = api.GetLibraryFiles();

            var artists = artistRepository.GetAll();
            var genres = genreRepository.GetAll();
            var albums = albumRepository.GetAll();

            var cached = trackRepository.GetCached();
            var deleted = trackRepository.GetDeleted();
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
                        trackRepository.SoftDelete(list);
                    }, exception => Logger.Debug(exception));

            Logger.Debug($"data to insert: {toInsert.Count}, data to delete: {toDelete.Count} ");

            var updatedAlbums = new List<LibraryAlbum>();

            var tracks = toInsert
                .Select(file =>
                {
                    var deletedTrack = deleted.FirstOrDefault(tr => tr.Path.Equals(file));

                    var meta = api.GetTags(file);

                    var genre = genres.SingleOrDefault(q => q.Name == meta.Genre);
                    var artist = artists.SingleOrDefault(q => q.Name == meta.Artist);
                    var albumArtist = artists.SingleOrDefault(q => q.Name == meta.AlbumArtist);
                    var album = albums.SingleOrDefault(q => q.Name == meta.Album);

                    if (album != null && albumArtist != null && album.ArtistId != albumArtist.Id)
                    {
                        album.ArtistId = albumArtist.Id;
                        updatedAlbums.Add(album);
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
                    return track;
                }).ToList();

            Logger.Debug($"Tracks: {tracks.Count()} entries.");
            trackRepository.Save(tracks);
            albumRepository.Save(updatedAlbums);

            Logger.Debug($"Tracks: {toInsert.Count()} entries inserted.");
            Logger.Debug($"Tracks: {toDelete.Count()} entries deleted.");
            Logger.Debug($"Updated {updatedAlbums.Count} album entries");
        }
    }
}