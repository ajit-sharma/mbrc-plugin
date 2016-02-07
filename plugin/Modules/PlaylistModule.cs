namespace MusicBeePlugin.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using MusicBeePlugin.AndroidRemote.Extensions;
    using MusicBeePlugin.ApiAdapters;
    using MusicBeePlugin.Comparers;
    using MusicBeePlugin.Repository;
    using MusicBeePlugin.Rest.ServiceInterface;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    using NLog;

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

        private readonly IPlaylistRepository playlistRepository;

        private readonly IPlaylistTrackInfoRepository trackInfoRepository;

        private readonly IPlaylistTrackRepository trackRepository;

        private readonly IPlaylistApiAdapter api;

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
        /// <returns>A <see cref="ResponseBase" /> is returned.</returns>
        public ResponseBase CreateNewPlaylist(string name, string[] list)
        {
            if (list == null)
            {
                list = new string[] { };
            }

            var path = this.api.CreatePlaylist(name, list);
            var playlist = new Playlist { Path = path, Name = name, Tracks = list.Count() };
            var id = this.playlistRepository.SavePlaylist(playlist);
            playlist.Id = id;

            // List has elements that have to be synced with the cache.
            if (list.Length > 0)
            {
                Task.Factory.StartNew(() => { this.SyncPlaylistDataWithCache(playlist); });
            }

            return new ResponseBase { Code = id > 0 ? ApiCodes.Success : ApiCodes.Failure };
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
                            // Playlist was changed and cache is out of sync.
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
            var playlists = this.playlistRepository.GetUpdatedPlaylists(offset, limit, after);
            var total = this.playlistRepository.GetPlaylistCount();
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

            // using (var db = _cHelper.GetDbConnection())
            // {
            // var join = new JoinSqlBuilder<PlaylistTrackInfo, PlaylistTrack>();
            // join = join.Join<PlaylistTrackInfo, PlaylistTrack>(pti => pti.Id, pt => pt.TrackInfoId,
            // trackInfo => new
            // {
            // trackInfo.Path,
            // trackInfo.Title,
            // trackInfo.Id,
            // trackInfo.Artist,
            // trackInfo.DateAdded,
            // trackInfo.DateUpdated
            // }, playlistTrack => new {playlistTrack.Position})
            // .Where<PlaylistTrack>(p => p.PlaylistId == playlist.Id && p.DateDeleted == 0)
            // .OrderBy<PlaylistTrack>(pt => pt.Position);
            // var sql = join.ToSql();
            // var result = db.Query<PlaylistTrackInfo>(sql);
            // list.AddRange(result);
            // }
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
            var playlistTracks = this.trackRepository.GetUpdatedPlaylistTracks(id, offset, limit, after);
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
            return api.GetPlaylistTracks(playlist.Path);
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
            var trackInfo = this.trackInfoRepository.GetUpdatedPlaylistTrackInfo(offset, limit, after);
            var total = this.trackInfoRepository.GetPlaylistTrackInfoCount();
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
                this.playlistRepository.SavePlaylist(playlist);
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
            var playlists = this.GetPlaylistsFromApi();
            var cachedPlaylists = this.GetCachedPlaylists();

            var playlistComparer = new PlaylistComparer();
            var playlistsToInsert = playlists.Except(cachedPlaylists, playlistComparer).ToList();
            var playlistsToRemove = cachedPlaylists.Except(playlists, playlistComparer).ToList();

            foreach (var playlist in playlistsToRemove)
            {
                playlist.DateDeleted = DateTime.UtcNow.ToUnixTime();
                cachedPlaylists.Remove(playlist);

                // db.UpdateOnly(new PlaylistTrack {DateDeleted = DateTime.UtcNow.ToUnixTime()},
                // o => o.Update(p => p.DateDeleted)
                // .Where(pl => pl.PlaylistId == playlist.Id));
            }

            foreach (var playlist in playlistsToInsert)
            {
                // db.Save(playlist);
                // playlist.Id = db.GetLastInsertId();
            }

            // We have new inserts so update.
            if (playlistsToInsert.Count > 0)
            {
                // SetPlaylistUpdated(db);
            }

            cachedPlaylists.AddRange(playlistsToInsert);

            if (playlistsToRemove.Count > 0)
            {
                // db.SaveAll(playlistsToRemove);
                // // Entries deleted so update
                // SetPlaylistUpdated(db);
            }

            Logger.Debug("Playlists: {0} entries inserted.", playlistsToInsert.Count);
            Logger.Debug("Playlists: {0} entries removed.", playlistsToRemove.Count);

            foreach (var cachedPlaylist in cachedPlaylists)
            {
                this.SyncPlaylistDataWithCache(cachedPlaylist);
            }
        }

        /// <summary>
        ///     Updates the Date of the Playlists last update.
        ///     This date should change if any change happens to any of the playlists.
        /// </summary>
        /// <param name="db"></param>
        private static void SetPlaylistUpdated(IDbConnection db)
        {
            // var dateCache = db.Select<LastUpdated>(lu => lu.Id == 1);
            // var cached = dateCache.Count > 0 ? dateCache[0] : new LastUpdated();
            // cached.PlaylistsUpdated = DateTime.UtcNow;
            // db.Save(cached);
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
        private static void StorePlaylistTrack(
            Playlist playlist, 
            IDbConnection db, 
            PlaylistTrackInfo track, 
            List<PlaylistTrackInfo> tiCache)
        {
            // long id;
            // if (tiCache.Contains(track))
            // {
            // var info = tiCache.Find(p => p.Path.Equals(track.Path));
            // id = info.Id;
            // // If the entry was previously soft deleted now the entry will be
            // // reused so we are remove the DateDeleted.
            // if (info.DateDeleted != 0)
            // {
            // info.DateDeleted = 0;
            // db.Save(info);
            // }
            // }
            // else
            // {
            // db.Save(track);
            // id = db.GetLastInsertId();
            // }
            // var trackPlay = new PlaylistTrack
            // {
            // PlaylistId = playlist.Id,
            // TrackInfoId = id,
            // Position = track.Position
            // };
            // db.Save(trackPlay);
        }

        /// <summary>
        ///     Checks for unused <see cref="PlaylistTrackInfo" /> entries and sets
        ///     the <see cref="TypeBase.DateDeleted" />property to the current UTC
        ///     DateTime. />
        /// </summary>
        private void CleanUnusedTrackInfo()
        {
            // using (var db = _cHelper.GetDbConnection())
            // {
            // var usedTrackInfoIds = db.Select<PlaylistTrack>()
            // .Select(track => track.TrackInfoId)
            // .ToList();
            // var storedTrackInfoIds = db.Select<PlaylistTrackInfo>()
            // .Select(track => track.Id)
            // .ToList();
            // var unused = storedTrackInfoIds.Except(usedTrackInfoIds);
            // using (var transaction = db.OpenTransaction())
            // {
            // foreach (var id in unused)
            // {
            // db.UpdateOnly(new PlaylistTrackInfo
            // {
            // DateDeleted = DateTime.UtcNow.ToUnixTime()
            // }, o => o.Update(p => p.DateDeleted)
            // .Where(p => p.Id == id));
            // }
            // transaction.Commit();
            // }
            // }
        }

        /// <summary>
        ///     Retrieves the playlists stored in the MusicBee Remote cache.
        /// </summary>
        /// <returns>A list of <see cref="Playlist" /> objects.</returns>
        private List<Playlist> GetCachedPlaylists()
        {
            return this.playlistRepository.GetCachedPlaylists().ToList();
        }

        /// <summary>
        ///     Retrieves a cached playlist by it's id.
        /// </summary>
        /// <param name="id">The id of a playlist</param>
        /// <returns>A <see cref="Playlist" /> object.</returns>
        private Playlist GetPlaylistById(int id)
        {
            return this.playlistRepository.GetPlaylist(id);
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
            Logger.Debug("Checking changes for playlist: {0}", playlist.Path);
            var tracksUpdated = 0;
            var cachedTracks = this.trackRepository.GetTracksForPlaylist(playlist.Id);

            var playlistTracks = this.GetPlaylistTracksFromApi(playlist);
            var cachedPlaylistTracks = this.GetCachedPlaylistTracks(playlist);

            var comparer = new PlaylistTrackInfoComparer();

            var tracksToInsert = playlistTracks.Except(cachedPlaylistTracks, comparer).ToList();
            var tracksToDelete = cachedPlaylistTracks.Except(playlistTracks, comparer).ToList();

            var duplicatesPaths =
                tracksToDelete.GroupBy(x => x.Path)
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key)
                    .ToList();

            foreach (var path in duplicatesPaths)
            {
                var duplicatesDeleted = tracksToDelete.FindAll(track => track.Path.Equals(path));

                foreach (var deleted in duplicatesDeleted)
                {
                    var inserted = tracksToInsert.Find(track => track.Path.Equals(path));
                    if (inserted == null)
                    {
                        continue;
                    }

                    tracksToDelete.Remove(deleted);
                    tracksToInsert.Remove(inserted);
                    var cached = cachedPlaylistTracks.Find(track => track.GetHashCode() == deleted.GetHashCode());
                    cached.Position = inserted.Position;

                    var updated = cachedTracks.First(track => track.Id == cached.Id);

                    if (updated == null)
                    {
                        continue;
                    }

                    updated.Position = cached.Position;
                    updated.DateUpdated = DateTime.UtcNow.ToUnixTime();

                    // Track has been updated so increment
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

                var updated = cachedTracks.First(cTrack => cTrack.Id == trackInfo.Id);

                if (updated == null)
                {
                    continue;
                }

                updated.Position = trackInfo.Position;
                updated.DateUpdated = DateTime.UtcNow.ToUnixTime();

                // Track has been updated so increment.
                tracksUpdated++;
            }

            // Reactivating
            comparer.IncludePosition = true;
            Logger.Debug(
                "{0} tracks inserted.\t {1} tracks deleted.\t {2} tracks updated.",
                tracksToInsert.Count(),
                tracksToDelete.Count(),
                tracksUpdated);

            foreach (var track in tracksToDelete)
            {
                var cachedTrack = cachedTracks.First(t => t.PlaylistId == playlist.Id && t.TrackInfoId == track.Id);
                if (cachedTrack == null)
                {
                    continue;
                }

                cachedTrack.DateDeleted = DateTime.UtcNow.ToUnixTime();
                cachedPlaylistTracks.Remove(track);
            }

            var tiCache = this.trackInfoRepository.GetAllPlaylistTrackInfo();

            foreach (var track in tracksToInsert)
            {
                // StorePlaylistTrack(playlist, db, track, tiCache);
            }

            cachedPlaylistTracks.Sort();

            Logger.Debug(
                "The playlists should be equal now: {0}",
                playlistTracks.SequenceEqual(cachedPlaylistTracks, comparer));

            if (tracksToInsert.Count + tracksToDelete.Count + tracksUpdated > 0)
            {
                playlist.DateUpdated = DateTime.UtcNow.ToUnixTime();

                // db.Save(playlist);
            }

            // db.SaveAll(cachedTracks);
        }
    }
}