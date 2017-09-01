using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Feature.Library;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;
using MusicBeeRemoteData.Entities;
using MusicBeeRemoteData.Extensions;
using MusicBeeRemoteData.Repository.Interfaces;
using Nelibur.ObjectMapper;
using NLog;

namespace MusicBeeRemote.Core.Modules
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
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IAlbumRepository _albumRepository;

        private readonly ILibraryApiAdapter _libraryAdapter;

        private readonly IArtistRepository _artistRepository;

        private readonly ICoverRepository _coverRepository;

        private readonly IGenreRepository _genreRepository;

        private readonly ITrackRepository _trackRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LibraryModule" /> class.
        /// </summary>
        public LibraryModule(
            ILibraryApiAdapter libraryAdapter,
            IGenreRepository genreRepository,
            IArtistRepository artistRepository,
            IAlbumRepository albumRepository,
            ITrackRepository trackRepository,
            ICoverRepository coverRepository)
        {
            _albumRepository = albumRepository;
            _trackRepository = trackRepository;
            _coverRepository = coverRepository;
            _genreRepository = genreRepository;
            _artistRepository = artistRepository;
            _libraryAdapter = libraryAdapter;
        }

        /// <summary>
        ///     Builds the artist cover cache.
        ///     Method is really slow, due to multiple threads being called.
        ///     Should be better called on a low priority thread.
        /// </summary>
        public void BuildArtistCoverCache()
        {
            var artists = _libraryAdapter.GetArtists();
            foreach (var artist in artists)
            {
                var name = artist.Name;
                var imageUrl = _libraryAdapter.GetArtistUrl(name);

                if (string.IsNullOrEmpty(imageUrl))
                {
                    continue;
                }

                var hash = Utilities.Utilities.CacheArtistImage(imageUrl, name);
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
            var albums = _albumRepository.GetAll();
            var cachedCovers = _coverRepository.GetAll();
            _logger.Debug($"Start cover processing, currently cached {cachedCovers.Count} entrie");

            var updatedAlbums = new List<AlbumDao>();

            var albumCovers = new Dictionary<AlbumDao, LibraryCover>();

            foreach (var album in albums)
            {
                var firstAlbumTrack = _trackRepository.GetFirstAlbumTrackPathById(album.Id);

                if (firstAlbumTrack == null)
                {
                    continue;
                }

                var coverUrl = _libraryAdapter.GetCoverUrl(firstAlbumTrack);
                string hash;

                if (string.IsNullOrEmpty(coverUrl))
                {
                    var data = _libraryAdapter.GetCover(firstAlbumTrack);
                    hash = Utilities.Utilities.StoreCoverToCache(data);
                }
                else
                {
                    hash = Utilities.Utilities.StoreCoverToCache(coverUrl);
                }

                if (string.IsNullOrEmpty(hash))
                {
                    continue;
                }

                var cached = cachedCovers.Select(libraryCover => libraryCover.Hash = hash).FirstOrDefault();

                if (cached != null)
                {
                    _logger.Debug($"Cached entry detected for {hash}");
                    continue;
                }

                var cover = new LibraryCover {Hash = hash};
                albumCovers.Add(album, cover);
            }

            var libraryCovers = albumCovers.Values.ToList();
            _coverRepository.Save(libraryCovers);

            foreach (var pair in albumCovers)
            {
                pair.Key.CoverId = pair.Value.Id;
            }

            var updated = _albumRepository.Save(albumCovers.Keys.ToList());
            _logger.Debug($"Albums updated {updated}, covers updated {updated}");
        }

        /// <summary>
        ///     Given an album track it will return the paths of the tracks
        ///     included in the album.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string[] GetAlbumTracksById(int id)
        {
            var trackList = new List<string>();
            var tracks = _trackRepository.GetTracksByAlbumId(id);
            trackList.AddRange(tracks.Select(track => track.Path));
            return trackList.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public PaginatedResponse<AlbumDao> GetAllAlbums(int limit, int offset, long after)
        {
            var albums = _albumRepository.GetUpdatedPage(offset, limit, after);
            var total = _albumRepository.GetCount();
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
        ///     Retrieves a number of <see cref="ArtistDao" /> results (a page) from the cache.
        /// </summary>
        /// <param name="limit">The number of results in the page.</param>
        /// <param name="offset">The first position in the result set.</param>
        /// <param name="after"></param>
        /// <returns></returns>
        public PaginatedResponse<ArtistDao> GetAllArtists(int limit, int offset, long after)
        {
            var artists = _artistRepository.GetUpdatedPage(offset, limit, after);
            var total = _artistRepository.GetCount();
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
            var updatedCovers = _coverRepository.GetUpdatedPage(offset, limit, after);
            var total = _coverRepository.GetCount();
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
        public PaginatedResponse<GenreDao> GetAllGenres(int limit, int offset, long after)
        {
            var genres = _genreRepository.GetUpdatedPage(offset, limit, after);
            var total = _genreRepository.GetCount();
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
        public PaginatedResponse<TrackDao> GetAllTracks(int limit, int offset, long after)
        {
            var tracks = _trackRepository.GetUpdatedPage(offset, limit, after);
            var total = _trackRepository.GetCount();
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
        /// <returns>The cached <see cref="ArtistDao" /> for the provided id.</returns>
        public ArtistDao GetArtistById(int id)
        {
            return _artistRepository.GetById(id);
        }

        public int GetCachedCoverCount()
        {
            return _coverRepository.GetCount();
        }

        public int GetCachedTrackCount()
        {
            return _trackRepository.GetCount();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Stream GetCoverData(int id)
        {
            var cover = GetLibraryCover(id);
            return Utilities.Utilities.GetCoverStreamFromCache(cover.Hash);
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeImage"></param>
        /// <returns></returns>
        public LibraryCover GetLibraryCover(int id, bool includeImage = false) => _coverRepository.GetById(id);

        /// <summary>
        ///     Retrieves a <see cref="TrackDao" /> from the cache using it's id.
        /// </summary>
        /// <param name="id">The id of the entry in the database.</param>
        /// <returns></returns>
        public TrackDao GetTrackById(int id) => _trackRepository.GetById(id);

        /// <summary>
        ///     Given an id in the database it will retrieve the path of the track.
        ///     It returns an array instead of a single String to be in consistency
        ///     with the other group of methods. <see cref="GetAlbumTracksById" /> etc.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string[] GetTrackPathById(int id)
        {
            var list = new List<string>();
            var track = _trackRepository.GetById(id);
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
            _libraryAdapter.GetSyncDelta(cachedFiles, lastSync);
        }

        /// <summary>
        ///     Checks for changes in the <see cref="AlbumDao" /> table.
        /// </summary>
        public void UpdateAlbumTable()
        {
            var artists = _artistRepository.GetAll().ToDictionary(dao => dao.Id, dao => dao.Name);

            var albums = _libraryAdapter.GetAlbums().Select(album =>
            {
                var info = TinyMapper.Map<CompareableAlbum>(album);
                return info;
            }).ToList();

            var cached = _albumRepository.GetCached().Select(dao =>
            {
                var info = TinyMapper.Map<CompareableAlbum>(dao);
                info.Artist = artists[dao.ArtistId] ?? "";
                return info;
            }).ToList();

            var deleted = _albumRepository.GetDeleted().Select(dao =>
            {
                var info = TinyMapper.Map<CompareableAlbum>(dao);
                info.Artist = artists[dao.ArtistId] ?? "";
                return info;
            }).ToList();

            var toInsert = albums.Except(cached, CompareableAlbum.NameArtistComparer)
                .Select(album =>
                {
                    var matched = deleted.FirstOrDefault(deletedAlbum => album.Artist.Equals(deletedAlbum.Artist)
                                                                         && album.Name.Equals(deletedAlbum.Name));
                    album.Id = matched?.Id ?? 0;
                    return album;
                })
                .Select(TinyMapper.Map<AlbumDao>)
                .ToList();


            var toRemove = cached.Except(albums, CompareableAlbum.NameArtistComparer)
                .Select(TinyMapper.Map<AlbumDao>)
                .ToList();

            _albumRepository.SoftDelete(toRemove);
            _logger.Debug($"Albums: {toRemove.Count} entries removed");


            _albumRepository.Save(toInsert);
            _logger.Debug($"Albums: {toInsert.Count} entries inserted");
        }

        /// <summary>
        ///     Checks for changes in the <see cref="ArtistDao" /> table.
        /// </summary>
        public void UpdateArtistTable()
        {
            var artists = _libraryAdapter.GetArtists();
            var cached = _artistRepository.GetCached();
            var deleted = _artistRepository.GetDeleted();

            var names = artists.Select(artist => artist.Name);
            var cachedNames = cached.Select(artist => artist.Name);

            var toInsert = artists.Where(artist => !cachedNames.Contains(artist.Name))
                .Select(artist =>
                {
                    var match = deleted.First(art => art.Name.Equals(
                        artist.Name,
                        StringComparison.InvariantCultureIgnoreCase
                    ));
                    var artistDao = TinyMapper.Map<ArtistDao>(artist);
                    artistDao.Id = match?.Id ?? 0;
                    return artistDao;
                })
                .ToList();

            var toDelete = cached.Where(artist => !names.Contains(artist.Name)).ToList();

            _artistRepository.SoftDelete(toDelete);
            _logger.Debug($"Artists: {toDelete.Count} entries deleted.");

            _artistRepository.Save(toInsert);
            _logger.Debug($"Artists: {toInsert.Count} entries inserted");
        }

        /// <summary>
        ///     Checks for changes in the <see cref="GenreDao" /> table.
        /// </summary>
        public void UpdateGenreTable()
        {
            var genres = _libraryAdapter.GetGenres();
            var cached = _genreRepository.GetCached();
            var deleted = _genreRepository.GetDeleted();

            var cachedNames = cached.Select(genre => genre.Name);
            var names = cached.Select(genre => genre.Name);

            var toInsert = genres.Where(genre => !cachedNames.Contains(genre.Name))
                .Select(genre =>
                {
                    var match = deleted.First(art => art.Name.Equals(
                        genre.Name,
                        StringComparison.InvariantCultureIgnoreCase
                    ));
                    var genreDao = TinyMapper.Map<GenreDao>(genre);
                    genreDao.Id = match?.Id ?? 0;
                    return genreDao;
                })
                .ToList();
            var toRemove = cached.Where(genre => !names.Contains(genre.Name)).ToList();

            _genreRepository.SoftDelete(toRemove);
            _logger.Debug($"Genres: {toRemove.Count} entries removed");

            _genreRepository.Save(toInsert);
            _logger.Debug($"Genres: {toInsert.Count} entries inserted");
        }

        /// <summary>
        ///     Checks for updates the <see cref="TrackDao" /> table.
        /// </summary>
        private void UpdateTrackTable()
        {
            var files = _libraryAdapter.GetLibraryFiles();

            var artists = _artistRepository.GetAll();
            var genres = _genreRepository.GetAll();
            var albums = _albumRepository.GetAll();

            var cached = _trackRepository.GetCached();
            var deleted = _trackRepository.GetDeleted();
            var cachedPaths = cached.Select(tr => tr.Path).ToList();

            var toInsert = files.Except(cachedPaths).ToArray();
            var toDelete = cachedPaths.Except(files).ToList();

            cached.ToObservable()
                .Where(s => toDelete.Contains(s.Path))
                .ToList()
                .DefaultIfEmpty(new List<TrackDao>())
                .Subscribe(
                    list =>
                    {
                        if (list.Count == 0)
                        {
                            return;
                        }
                        _trackRepository.SoftDelete(list);
                    }, exception => _logger.Debug(exception));

            _logger.Debug($"data to insert: {toInsert.Length}, data to delete: {toDelete.Count} ");

            var updatedAlbums = new List<AlbumDao>();

            
            var tracks = _libraryAdapter.GetTracks(toInsert)
                .Select(file =>
                {
                    var deletedTrack = deleted.FirstOrDefault(tr => tr.Path.Equals(file.Url));
                   
                    var genre = genres.SingleOrDefault(q => q.Name == file.Genre);
                    var artist = artists.SingleOrDefault(q => q.Name == file.Artist);
                    var albumArtist = artists.SingleOrDefault(q => q.Name == file.AlbumArtist);
                    var album = albums.SingleOrDefault(q => q.Name == file.Album);

                    if (album != null && albumArtist != null && album.ArtistId != albumArtist.Id)
                    {
                        album.ArtistId = albumArtist.Id;
                        updatedAlbums.Add(album);
                    }

                    var track = new TrackDao
                    {
                        Title = file.Title,
                        Year = file.Year,
                        Position = file.TrackNo,
                        Disc = file.Disc,
                        GenreId = genre?.Id ?? 0,
                        AlbumArtistId = albumArtist?.Id ?? 0,
                        ArtistId = artist?.Id ?? 0,
                        AlbumId = album?.Id ?? 0,
                        Path = file.Url
                    };

                    // Current track was detected in the list of deleted tracks.
                    // So we are going to update the existing entry.
                    if (deletedTrack == null)
                    {
                        return track;
                    }
                    track.Id = deletedTrack.Id;
                    track.DateUpdated = DateTime.UtcNow.ToUnixTime();
                    track.DateDeleted = 0;
                    return track;
                }).ToList();

            _logger.Debug($"Tracks: {tracks.Count} entries.");
            _trackRepository.Save(tracks);
            _albumRepository.Save(updatedAlbums);

            _logger.Debug($"Tracks: {toInsert.Length} entries inserted.");
            _logger.Debug($"Tracks: {toDelete.Count} entries deleted.");
            _logger.Debug($"Updated {updatedAlbums.Count} album entries");
        }
    }
}