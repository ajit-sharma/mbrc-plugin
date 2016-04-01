namespace MusicBeeRemoteCore.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    using MusicBeeRemoteCore.ApiAdapters;
    using MusicBeeRemoteCore.Comparers;
    using MusicBeeRemoteCore.Rest.ServiceInterface;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Extensions;
    using MusicBeeRemoteData.Repository.Interfaces;

    using NLog;

    /// <summary>
    ///     This module is responsible for the playlist functionality.
    ///     It implements the playlist operation with the MusicBee API and the
    ///     plugin cache.
    /// </summary>
    public class PlaylistModule
    {
        /// <summary>
        ///     The logger is used to log errors.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IPlaylistApiAdapter api;

        private readonly IPlaylistRepository playlistRepository;

        private readonly IPlaylistTrackInfoRepository trackInfoRepository;

        private readonly IPlaylistTrackRepository trackRepository;

        /// <summary>
        ///     Creates a new <see cref="PlaylistModule" />.
        /// </summary>
        /// <param name="api"></param>
        public PlaylistModule(
            IPlaylistApiAdapter api, 
            IPlaylistRepository playlistRepository, 
            IPlaylistTrackRepository trackRepository, 
            IPlaylistTrackInfoRepository trackInfoRepository)
        {
            this.playlistRepository = playlistRepository;
            this.trackRepository = trackRepository;
            this.trackInfoRepository = trackInfoRepository;
            this.api = api;
        }

        /// <summary>
        ///     Creates a new playlist.
        /// </summary>
        /// <param name="name">The name of the playlist that will be created.</param>
        /// <param name="list">
        ///     A string list containing the full paths of the tracks
        ///     that will be added to the playlist. If left empty an empty playlist will be created.
        /// </param>
        /// <returns>True if successfully completed and false otherwise.</returns>
        public bool CreateNewPlaylist(string name, string[] list)
        {
            if (list == null)
            {
                list = new string[] { };
            }

            var path = this.api.CreatePlaylist(name, list);

            var created = !string.IsNullOrEmpty(path);

            if (!created)
            {
                return false;
            }

            Observable.Create<Playlist>(
                observer =>
                    {
                        var playlist = new Playlist { Path = path, Name = name, Tracks = list.Count() };
                        var id = this.playlistRepository.Save(playlist);
                        playlist.Id = id;
                        observer.OnNext(playlist);
                        observer.OnCompleted();
                        return () => { };
                    }).SelectMany(
                        playlist =>
                            {
                                // List has elements that have to be synced with the cache.
                                if (list.Length > 0)
                                {
                                    return Observable.Create<string>(
                                        o =>
                                            {
                                                this.SyncPlaylistDataWithCache(playlist);
                                                o.OnCompleted();
                                                return () => { };
                                            });
                                }
                                else
                                {
                                    return Observable.Empty<string>();
                                }
                            })
                .SubscribeOn(ThreadPoolScheduler.Instance)
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Subscribe(s => { }, exception => Logger.Debug(exception, "During playlist sync"));

            return true;
        }

        /// <summary>
        ///     Removes a track from the specified playlist.
        /// </summary>
        /// <param name="id">The <c>id</c> of the playlist</param>
        /// <param name="position">The <c>position</c> of the track in the playlist</param>
        /// <returns></returns>
        public bool DeleteTrackFromPlaylist(int id, int position)
        {
            var playlist = this.GetPlaylistById(id);
            var success = this.api.RemoveTrack(playlist.Path, position);
            if (success)
            {
                Task.Factory.StartNew(
                    () =>
                        {
                            // Playlist was Changed and cache is out of sync.
                            // So we start a task to update the cache.
                            this.SyncPlaylistDataWithCache(playlist);
                        });
            }

            return success;
        }

        /// <summary>
        ///     Retrieves a page of <see cref="Playlist" /> results.
        /// </summary>
        /// <param name="limit">The number of entries in the page.</param>
        /// <param name="offset">The index of the first result in the page.</param>
        /// <param name="after">The date threshold after which the data are required</param>
        /// <returns>A PaginatedResponse containing playlists</returns>
        public PaginatedResponse<Playlist> GetAvailablePlaylists(int limit = 50, int offset = 0, long after = 0)
        {
            var playlists = this.playlistRepository.GetUpdatedPage(offset, limit, after);
            var total = this.playlistRepository.GetCount();
            var paginated = new PaginatedPlaylistResponse
                                {
                                    Total = total, 
                                    Limit = limit, 
                                    Offset = offset, 
                                    Data = playlists.ToList()
                                };
            return paginated;
        }

        /// <summary>
        ///     Gets the cached <c>playlist</c> tracks ordered by the position in the <c>playlist</c>.
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        public List<PlaylistTrackInfo> GetCachedPlaylistTracks(Playlist playlist)
        {
            var list = new List<PlaylistTrackInfo>();

            return list;
        }

        /// <summary>
        ///     Retrieves a page of <see cref="PlaylistTrack" /> results.
        /// </summary>
        /// <param name="id">The id of the playlist that contains the tracks.</param>
        /// <param name="limit">The number of the results in the page.</param>
        /// <param name="offset">The index of the first result in the page.</param>
        /// <param name="after"></param>
        /// <returns></returns>
        public PaginatedResponse<PlaylistTrack> GetPlaylistTracks(
            int id, 
            int limit = 50, 
            int offset = 0, 
            long after = 0)
        {
            var playlistTracks = this.trackRepository.GetUpdatedTracksForPlaylist(id, offset, limit, after);
            var total = this.trackRepository.GetTrackCountForPlaylist(id);
            var paginated = new PaginatedPlaylistTrackResponse
                                {
                                    Total = total, 
                                    Limit = limit, 
                                    Offset = offset, 
                                    Data = playlistTracks.ToList()
                                };
            return paginated;
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
            return this.api.GetPlaylistTracks(playlist.Path);
        }

        /// <summary>
        ///     Retrieves a page of <see cref="PlaylistTrackInfo" /> results.
        /// </summary>
        /// <param name="limit">The number of the results in the page.</param>
        /// <param name="offset">The index of the first result in the page.</param>
        /// <param name="after"></param>
        /// <returns></returns>
        public PaginatedResponse<PlaylistTrackInfo> GetPlaylistTracksInfo(
            int limit = 50, 
            int offset = 0, 
            long after = 0)
        {
            var trackInfo = this.trackInfoRepository.GetUpdatedPage(offset, limit, after);
            var total = this.trackInfoRepository.GetCount();
            var paginated = new PaginatedPlaylistTrackInfoResponse
                                {
                                    Total = total, 
                                    Limit = limit, 
                                    Offset = offset, 
                                    Data = trackInfo.ToList()
                                };
            return paginated;
        }

        /// <summary>
        ///     Moves a track in a playlist to a new position in the playlist.
        /// </summary>
        /// <param name="id">The id of the playlist.</param>
        /// <param name="from">The original position of the track in the playlist.</param>
        /// <param name="to">The new position of the track in the playlist.</param>
        /// <returns></returns>
        public bool MovePlaylistTrack(int id, int from, int to)
        {
            var playlist = this.GetPlaylistById(id);
            var success = this.api.MoveTrack(playlist.Path, from, to);
            if (success)
            {
                Task.Factory.StartNew(() => { this.SyncPlaylistDataWithCache(playlist); });
            }

            return success;
        }

        /// <summary>
        ///     Adds tracks to an existing playlist.
        /// </summary>
        /// <param name="id">The id of the playlist.</param>
        /// <param name="list">A list of the paths of the files in the filesystem.</param>
        /// <returns></returns>
        public bool PlaylistAddTracks(int id, string[] list)
        {
            var playlist = this.GetPlaylistById(id);
            var success = this.api.AddTracks(playlist.Path, list);
            if (success)
            {
                Task.Factory.StartNew(() => { this.SyncPlaylistDataWithCache(playlist); });
            }

            return success;
        }

        /// <summary>
        ///     Deletes a playlist.
        /// </summary>
        /// <param name="id">The id of the playlist to delete.</param>
        /// <returns></returns>
        public bool PlaylistDelete(int id)
        {
            var playlist = this.GetPlaylistById(id);
            var success = this.api.DeletePlaylist(playlist.Path);
            if (success)
            {
                playlist.DateDeleted = DateTime.UtcNow.ToUnixTime();
                this.playlistRepository.Save(playlist);
            }

            return success;
        }

        /// <summary>
        ///     Given the hash representing of a playlist it plays the specified playlist.
        /// </summary>
        /// <param name="path">The playlist path</param>
        public ResponseBase PlaylistPlayNow(string path)
        {
            return new ResponseBase { Code = this.api.PlayNow(path) ? ApiCodes.Success : ApiCodes.Failure };
        }

        /// <summary>
        ///     Syncs the playlist information in the cache with the information available
        ///     from the MusicBee API.
        /// </summary>
        /// <returns></returns>
        public void SyncPlaylistsWithCache()
        {
            Logger.Debug("Starting playlist sync");
            var playlists = this.GetPlaylistsFromApi();
            var cachedPlaylists = this.GetCachedPlaylists();

            var playlistComparer = new PlaylistComparer();
            var playlistsToInsert = playlists.Except(cachedPlaylists, playlistComparer).ToList();
            var playlistsToRemove = cachedPlaylists.Except(playlists, playlistComparer).ToList();

            this.playlistRepository.SoftDelete(playlistsToRemove);

            var deletedIds = playlistsToRemove.Select(playlist => playlist.Id).ToList();

            this.trackRepository.DeleteTracksForPlaylists(deletedIds);
            cachedPlaylists = cachedPlaylists.Except(playlistsToRemove).ToList();

            this.playlistRepository.Save(playlistsToInsert);
            cachedPlaylists.AddRange(playlistsToInsert);

            Logger.Debug($"Playlists: {playlistsToInsert.Count} entries inserted.");
            Logger.Debug($"Playlists: {playlistsToRemove.Count} entries removed.");

            foreach (var cachedPlaylist in cachedPlaylists)
            {
                this.SyncPlaylistDataWithCache(cachedPlaylist);
            }

            this.CleanUnusedTrackInfo();
        }

        /// <summary>
        ///     Checks for unused <see cref="PlaylistTrackInfo" /> entries and sets
        ///     the <see cref="Rest.ServiceModel.Type.TypeBase.DateDeleted" />property to the current UTC
        ///     DateTime. />
        /// </summary>
        private void CleanUnusedTrackInfo()
        {
            var usedIds = this.trackRepository.GetUsedTrackInfoIds();
            var allIds = this.trackInfoRepository.GetAllIds();
            var unused = allIds.Except(usedIds).ToList();
            var deletedUnused = this.trackInfoRepository.SoftDeleteUnused(unused);

            Logger.Debug(
                $"Out of {allIds.Count} total track info entries {usedIds.Count} ids used, {unused.Count} unused");
            Logger.Debug($"Soft deleted a total of {deletedUnused} unused entries");
        }

        /// <summary>
        ///     Retrieves the playlists stored in the MusicBee Remote cache.
        /// </summary>
        /// <returns>A list of <see cref="Playlist" /> objects.</returns>
        private List<Playlist> GetCachedPlaylists()
        {
            return this.playlistRepository.GetCached().ToList();
        }

        /// <summary>
        ///     Retrieves a cached playlist by it's id.
        /// </summary>
        /// <param name="id">The id of a playlist</param>
        /// <returns>A <see cref="Playlist" /> object.</returns>
        private Playlist GetPlaylistById(int id)
        {
            return this.playlistRepository.GetById(id);
        }

        /// <summary>
        ///     Retrieves the playlists from the MusicBee API.
        /// </summary>
        /// <returns>A list of <see cref="Playlist" /> objects.</returns>
        private List<Playlist> GetPlaylistsFromApi()
        {
            return this.api.GetPlaylists();
        }

        /// <summary>
        ///     Syncs the <see cref="PlaylistTrack" /> cache with the data available
        ///     from the MusicBee API.
        /// </summary>
        /// <param name="playlist">The playlist for which the sync happens</param>
        private void SyncPlaylistDataWithCache(Playlist playlist)
        {
            Logger.Debug($"Checking changes for playlist: {playlist.Path}");
           

            var playlistTracks = this.trackRepository.GetTracksForPlaylist(playlist.Id);
            var currentTracks = this.GetPlaylistTracksFromApi(playlist);
            var storedTracks = this.trackInfoRepository.GetTracksForPlaylist((int)playlist.Id);
            var cachedInfo = this.trackInfoRepository.GetAll();

            var comparer = new PlaylistTrackInfoComparer { IncludePosition = false };

            var tracksToInsert = currentTracks.Except(storedTracks, comparer).ToList();
            var tracksToDelete = storedTracks.Except(currentTracks, comparer).ToList();

            var duplicates = currentTracks.GroupBy(track => track.Path).SelectMany(group => group.Skip(1)).Distinct().ToList();
            duplicates.ForEach(
                info =>
                {
                    var actualCount = currentTracks.Count(trackInfo => trackInfo.Path.Equals(info.Path));
                    var storedCount = storedTracks.Count(trackInfo => trackInfo.Path.Equals(info.Path));
                    var toInsertCount = tracksToInsert.Count(trackInfo => trackInfo.Path.Equals(info.Path));

                    Logger.Debug($"stored instances {storedCount} and actual instances {actualCount}");

                    var times = actualCount - storedCount - toInsertCount;

                    if (times > 0)
                    {
                        for (var i = 0; i < times; i++)
                        {
                            tracksToInsert.Add(info);
                        }
                    }
                    else
                    {
                        times = Math.Abs(times);
                        for (var i = 0; i < times; i++)
                        {
                            var track = storedTracks.FirstOrDefault(trackInfo => trackInfo.Path.Equals(info.Path));
                            if (track != null)
                            {
                                tracksToDelete.Add(track);
                            }                            
                        }
                    }
                });
                    
            

            var missing = tracksToInsert.Where(info => !cachedInfo.Contains(info, comparer)).ToList();
            this.trackInfoRepository.Save(missing);

            var removedIds = tracksToDelete.Select(track => track.Id).ToList();
            var toRemove = playlistTracks.Where(track => removedIds.Contains(track.Id)).ToList();

            if (missing.Count > 0)
            {
                Logger.Debug($"Had {missing.Count} entries refreshing cached info.");
                cachedInfo.Clear();
                cachedInfo = this.trackInfoRepository.GetAll();
            }

            this.trackRepository.Delete(toRemove);

            var tracks =
                tracksToInsert.Select(
                    info =>
                    new PlaylistTrack()
                        {
                            PlaylistId = playlist.Id, 
                            TrackInfoId =
                                cachedInfo.Where(trackInfo => trackInfo.Path.Equals(info.Path))
                                .Select(trackInfo => trackInfo.Id)
                                .FirstOrDefault(), 
                            Position = info.Position
                        }).ToList();

            this.trackRepository.Save(tracks);

            var stored = this.trackInfoRepository.GetTracksForPlaylist(playlist.Id);
            var updated = new List<PlaylistTrack>();

            foreach (var info in stored)
            {
                var first = currentTracks.FirstOrDefault(trackInfo => trackInfo.Path.Equals(info.Path));
                if (first == null)
                {
                    continue;
                }

                if (first.Position != info.Position)
                {
                    Logger.Debug($"Position missmatch should be at {first.Position} but found at {info.Position} instead");
                    
                    var playlistTrack = playlistTracks.FirstOrDefault(track => track.Id == info.Id);
                    if (playlistTrack != null)
                    {
                        playlistTrack.Position = first.Position;
                        updated.Add(playlistTrack);
                    }
                    
                }
                currentTracks.Remove(first);
            }

            this.trackRepository.Save(updated);

            comparer.IncludePosition = true;

#if DEBUG
            this.CheckIfSynced(playlist);
#endif

            if (tracksToInsert.Count + tracksToDelete.Count + updated.Count > 0)
            {
                this.playlistRepository.Save(playlist);
            }
        }

#if DEBUG

        /// <summary>
        /// Checks if a playlist's stored data is in sync with the actual playlist data.
        /// </summary>
        /// <param name="playlist">The playlist</param>
        public void CheckIfSynced(Playlist playlist)
        {
            var comparer = new PlaylistTrackInfoComparer { IncludePosition = true };
            var currentTracks = this.GetPlaylistTracksFromApi(playlist);
            var storedTracks = this.trackInfoRepository.GetTracksForPlaylist((int)playlist.Id);
            Logger.Debug($"The playlists should be equal now: {currentTracks.SequenceEqual(storedTracks, comparer)}");
            Logger.Debug(currentTracks);
            Logger.Debug(storedTracks);
        }

        public object GetDebugPlaylistData()
        {
            var pl = this.GetCachedPlaylists().Last();
            return
                new
                    {
                        currentTracks = this.GetPlaylistTracksFromApi(pl), 
                        storedTracks = this.trackInfoRepository.GetTracksForPlaylist((int)pl.Id)
                    };
        }


        public void SyncDebugLastPlaylist()
        {
            if (this.playlistRepository.GetCount() == 0)
            {
                this.SyncPlaylistsWithCache();
                return;
            }

            var pl = this.GetCachedPlaylists().Last();
            this.SyncPlaylistDataWithCache(pl);
        }

#endif

    }
}