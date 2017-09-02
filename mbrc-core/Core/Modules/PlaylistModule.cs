using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Comparers;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;
using MusicBeeRemote.Data.Entities;
using MusicBeeRemote.Data.Extensions;
using MusicBeeRemote.Data.Repository.Interfaces;
using NLog;

namespace MusicBeeRemote.Core.Modules
{
    /// <summary>
    ///     This module is responsible for the playlist functionality.
    ///     It implements the playlist operation with the MusicBee API and the
    ///     plugin cache.
    /// </summary>
    public class PlaylistModule : IPlaylistModule
    {
        /// <summary>
        ///     The logger is used to log errors.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IPlaylistApiAdapter _api;

        private readonly IPlaylistRepository _playlistRepository;

        private readonly IPlaylistTrackInfoRepository _trackInfoRepository;

        private readonly IPlaylistTrackRepository _trackRepository;


        private readonly IScheduler _threadPoolScheduler;

        /// <summary>
        ///     Creates a new <see cref="PlaylistModule" />.
        /// </summary>
        /// <param name="api"></param>
        public PlaylistModule(
            IPlaylistApiAdapter api, 
            IPlaylistRepository playlistRepository, 
            IPlaylistTrackRepository trackRepository, 
            IPlaylistTrackInfoRepository trackInfoRepository, 
            IScheduler threadPoolScheduler)
        {
            _playlistRepository = playlistRepository;
            _trackRepository = trackRepository;
            _trackInfoRepository = trackInfoRepository;
            _threadPoolScheduler = threadPoolScheduler;
            _api = api;
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
            var path = _api.CreatePlaylist(name, list ?? new string[] {});
            
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            SyncPlaylistWithRepository(name, list, path);

            return true;
        }

        private void SyncPlaylistWithRepository(string name, ICollection<string> list, string path)
        {
            Observable.Create<Playlist>(
                observer =>
                {
                    var playlist = new Playlist {Url = path, Name = name, Tracks = list.Count()};
                    var id = _playlistRepository.Save(playlist);
                    playlist.Id = id;
                    observer.OnNext(playlist);
                    observer.OnCompleted();
                    return () => { };
                }).SelectMany(playlist => list.Count > 0 ? SyncPlaylistObservable(playlist) : Observable.Empty<string>())
                .SubscribeOn(_threadPoolScheduler)
                .ObserveOn(_threadPoolScheduler)
                .Subscribe(s => { }, exception => Logger.Debug(exception, "During playlist sync"));
        }

        private IObservable<string> SyncPlaylistObservable(Playlist playlist)
        {
            return Observable.Create<string>(
                o =>
                {
                    SyncPlaylistDataWithCache(playlist);
                    o.OnCompleted();
                    return () => { };
                }).SubscribeOn(_threadPoolScheduler)
                .ObserveOn(_threadPoolScheduler);
        }

        /// <summary>
        ///     Removes a track from the specified playlist.
        /// </summary>
        /// <param name="id">The <c>id</c> of the playlist</param>
        /// <param name="position">The <c>position</c> of the track in the playlist</param>
        /// <returns></returns>
        public bool DeleteTrackFromPlaylist(int id, int position)
        {
            var playlist = _playlistRepository.GetById(id);
            var success = _api.RemoveTrack(playlist.Url, position);
            if (success)
            {
                ExecuteSyncTask(playlist);
            }

            return success;
        }

        private void ExecuteSyncTask(Playlist playlist)
        {
            SyncPlaylistObservable(playlist)
                .Subscribe(s => { }, exception => { Logger.Debug(exception, "While deleting playlist"); });
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
            var playlists = _playlistRepository.GetUpdatedPage(offset, limit, after);
            var total = _playlistRepository.GetCount();
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
            var playlistTracks = _trackRepository.GetUpdatedTracksForPlaylist(id, offset, limit, after);
            var total = _trackRepository.GetTrackCountForPlaylist(id);
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
            var trackInfo = _trackInfoRepository.GetUpdatedPage(offset, limit, after);
            var total = _trackInfoRepository.GetCount();
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
            var playlist = _playlistRepository.GetById(id);
            var success = _api.MoveTrack(playlist.Url, from, to);
            if (success)
            {
                ExecuteSyncTask(playlist);
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
            var playlist = _playlistRepository.GetById(id);
            var success = _api.AddTracks(playlist.Url, list);
            if (success)
            {
                ExecuteSyncTask(playlist);
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
            var playlist = _playlistRepository.GetById(id);
            var success = _api.DeletePlaylist(playlist.Url);
            if (success)
            {
                playlist.DateDeleted = DateTime.UtcNow.ToUnixTime();
                _playlistRepository.Save(playlist);
            }

            return success;
        }

        /// <summary>
        ///     Given the hash representing of a playlist it plays the specified playlist.
        /// </summary>
        /// <param name="path">The playlist path</param>
        public bool PlaylistPlayNow(string path)
        {
            return _api.PlayNow(path);
        }

        /// <summary>
        ///     Syncs the playlist information in the cache with the information available
        ///     from the MusicBee API.
        /// </summary>
        /// <returns></returns>
        public void SyncPlaylistsWithCache()
        {
            Logger.Debug("Starting playlist sync");
            var playlists = _api.GetPlaylists();
            var cachedPlaylists = _playlistRepository.GetCached().ToList();

            var playlistComparer = new PlaylistComparer();
            var playlistsToInsert = playlists.Except(cachedPlaylists, playlistComparer).ToList();
            var playlistsToRemove = cachedPlaylists.Except(playlists, playlistComparer).ToList();

            _playlistRepository.SoftDelete(playlistsToRemove);

            var deletedIds = playlistsToRemove.Select(playlist => playlist.Id).ToList();

            _trackRepository.DeleteTracksForPlaylists(deletedIds);
            cachedPlaylists = cachedPlaylists.Except(playlistsToRemove).ToList();

            _playlistRepository.Save(playlistsToInsert);
            cachedPlaylists.AddRange(playlistsToInsert);

            Logger.Debug($"Playlists: {playlistsToInsert.Count} entries inserted.");
            Logger.Debug($"Playlists: {playlistsToRemove.Count} entries removed.");

            foreach (var cachedPlaylist in cachedPlaylists)
            {
                SyncPlaylistDataWithCache(cachedPlaylist);
            }

            CleanUnusedTrackInfo();
        }

        /// <summary>
        ///     Checks for unused <see cref="PlaylistTrackInfo" /> entries and sets
        ///     the <see cref="Rest.ServiceModel.Type.TypeBase.DateDeleted" />property to the current UTC
        ///     DateTime. />
        /// </summary>
        private void CleanUnusedTrackInfo()
        {
            var usedIds = _trackRepository.GetUsedTrackInfoIds();
            var allIds = _trackInfoRepository.GetAllIds();
            var unused = allIds.Except(usedIds).ToList();
            var deletedUnused = _trackInfoRepository.SoftDeleteUnused(unused);

            Logger.Debug(
                $"Out of {allIds.Count} total track info entries {usedIds.Count} ids used, {unused.Count} unused");
            Logger.Debug($"Soft deleted a total of {deletedUnused} unused entries");
        }

        /// <summary>
        ///     Syncs the <see cref="PlaylistTrack" /> cache with the data available
        ///     from the MusicBee API.
        /// </summary>
        /// <param name="playlist">The playlist for which the sync happens</param>
        /// <returns>Should return if the resulting playlists are the same sequencially</returns>
        public bool SyncPlaylistDataWithCache(Playlist playlist)
        {
            Logger.Debug($"Checking changes for playlist: {playlist.Url}");
           
            var playlistTracks = _trackRepository.GetTracksForPlaylist(playlist.Id);
            var currentTracks = _api.GetPlaylistTracks(playlist.Url);
            var storedTracks = _trackInfoRepository.GetTracksForPlaylist(playlist.Id);
            var cachedInfo = _trackInfoRepository.GetAll();

            var basicComparer = new PlaylistTrackInfoComparer { IncludePosition = false };
            var positionComparer = new PlaylistTrackInfoComparer { IncludePosition = true };

            var tracksToInsert = currentTracks.Except(storedTracks, basicComparer).ToList();
            var tracksToDelete = storedTracks.Except(currentTracks, basicComparer).ToList();

            var duplicates = currentTracks.GroupBy(track => track.Path).SelectMany(group => group.Skip(1)).Distinct().ToList();
            var previouslyMatched = new List<PlaylistTrackInfo>();
            
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
                            var match = currentTracks.Where(trackInfo => !previouslyMatched.Contains(trackInfo, positionComparer))
                            .FirstOrDefault(trackInfo => trackInfo.Path.Equals(info.Path));
                            if (match != null)
                            {
                                previouslyMatched.Add(match);
                            }
                            
                            tracksToInsert.Add(match);
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
                    
                   
            var missing = tracksToInsert.Where(info => !cachedInfo.Contains(info, basicComparer)).ToList();
            _trackInfoRepository.Save(missing);

            var removedIds = tracksToDelete.Select(track => track.Id).ToList();
            var toRemove = playlistTracks.Where(track => removedIds.Contains(track.Id)).ToList();

            if (missing.Count > 0)
            {
                Logger.Debug($"Had {missing.Count} entries refreshing cached info.");
                cachedInfo.Clear();
                cachedInfo = _trackInfoRepository.GetAll();
            }

            _trackRepository.Delete(toRemove);

            var matched = MatchWithExistingTrackInfo(playlist.Id, tracksToInsert, cachedInfo);

            _trackRepository.Save(matched);

            var moved = FindMovedTracks(playlist.Id, currentTracks);

            _trackRepository.Save(moved);
            
            if (tracksToInsert.Count + tracksToDelete.Count + moved.Count > 0)
            {
                _playlistRepository.Save(playlist);
            }

            return CheckIfSynced(playlist);
        }

        private static List<PlaylistTrack> MatchWithExistingTrackInfo(int playlistId, IList<PlaylistTrackInfo> tracksToInsert, IList<PlaylistTrackInfo> cachedInfo)
        {
            return tracksToInsert.Select(
                info =>
                    new PlaylistTrack
                    {
                        PlaylistId = playlistId, 
                        TrackInfoId = FindTrackInfoId(cachedInfo, info), 
                        Position = info.Position
                    }).ToList();
        }

        private static int FindTrackInfoId(IList<PlaylistTrackInfo> cachedInfo, PlaylistTrackInfo info)
        {
            return cachedInfo.Where(trackInfo => trackInfo.Path.Equals(info.Path))
                .Select(trackInfo => trackInfo.Id)
                .FirstOrDefault();
        }

        private List<PlaylistTrack> FindMovedTracks(int playlistId, IList<PlaylistTrackInfo> currentTracks)
        {           
            var stored = _trackInfoRepository.GetTracksForPlaylist(playlistId);
            
            var playlistTracks = _trackRepository.GetTracksForPlaylist(playlistId);
            var tracks = currentTracks.ToList();
           
            var moved = new List<PlaylistTrack>();
        
            foreach (var info in stored)
            {              
                var first = tracks.FirstOrDefault(trackInfo => trackInfo.Path.Equals(info.Path));
                if (first == null)
                {
                    continue;
                }

                if (first.Position != info.Position)
                {
                    Logger.Debug($"Position missmatch should be at {first.Position} but found at {info.Position} instead");

                    var existing = playlistTracks.FirstOrDefault(track => track.Id == info.Id);
                    if (existing != null)
                    {
                        existing.Position = first.Position;
                        moved.Add(existing);
                        playlistTracks.Remove(existing);
                    }
                }
                tracks.Remove(first);
            }

            Logger.Debug("moved: " + string.Join("\n", moved));
            return moved;
        }


        /// <summary>
        /// Checks if a playlist's stored data is in sync with the actual playlist data.
        /// </summary>
        /// <param name="playlist">The playlist</param>
        private bool CheckIfSynced(Playlist playlist)
        {
            var comparer = new PlaylistTrackInfoComparer { IncludePosition = true };
            var currentTracks = _api.GetPlaylistTracks(playlist.Url);
            var storedTracks = _trackInfoRepository.GetTracksForPlaylist(playlist.Id);
            var sequenceEqual = currentTracks.SequenceEqual(storedTracks, comparer);
            Logger.Debug($"The playlists should be equal now: {sequenceEqual}");
            Logger.Debug(string.Join("\n", currentTracks));
            Logger.Debug(string.Join("\n", storedTracks));
            return sequenceEqual;
            
        }
    }
}