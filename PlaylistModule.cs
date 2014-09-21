#region

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

namespace MusicBeePlugin
{
    public class PlaylistModule
    {
        private readonly CacheHelper _mHelper;
        private Plugin.MusicBeeApiInterface _api;

        public PlaylistModule(Plugin.MusicBeeApiInterface api, string storagePath)
        {
            _api = api;
            _mHelper = new CacheHelper(storagePath);
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

            using (var db = _mHelper.GetDbConnection())
            {
                db.SaveAll(GetNewPlaylists(playlists));
            }
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

                if (String.IsNullOrEmpty(path)) break;
                var playlist = new Playlist
                {
                    Path = path
                };
                playlists.Add(playlist);
            }
            return playlists;
        }

        private List<Playlist> GetCachedPlaylists()
        {
            using (var db = _mHelper.GetDbConnection())
            {
                return db.Select<Playlist>();
            }
        }

        public PaginatedResponse GetAvailablePlaylists(int limit = 50, int offset = 0)
        {
            var playlists = GetCachedPlaylists();
            var total = playlists.Count;
            var result = new PaginatedResponse
            {
                Limit = limit,
                Offset = offset,
                Data = playlists,
                Total = total
            };
            return result;
        }


        public PaginatedResponse GetPlaylistTracks(int id, int limit = 50, int offset = 0)
        {
            string[] pathList = {};
            var playlist = GetPlaylistById(id);

            if (!_api.Playlist_QueryFilesEx(playlist.Path, ref pathList))
            {
                return new PaginatedResponse();
            }

            var index = 0;
            var playlistTracks = pathList.Select(path => new PlaylistTrack
            {
                Index = index++,
                Artist = _api.Library_GetFileTag(path, Plugin.MetaDataType.Artist),
                Title = _api.Library_GetFileTag(path, Plugin.MetaDataType.TrackTitle),
                Path = path,
                PlaylistId = id
            }).ToList();

            return PaginatedResponse.GetPaginatedData(limit, offset, playlistTracks);
        }

        /// <summary>
        ///     Given the hash representing of a playlist it plays the specified playlist.
        /// </summary>
        /// <param name="path">The playlist path</param>
        public SuccessResponse PlaylistPlayNow(string path)
        {
            return new SuccessResponse
            {
                success = _api.Playlist_PlayNow(path)
            };
        }

        public bool DeleteTrackFromPlaylist(int id, int index)
        {
            var playlist = GetPlaylistById(id);
            return _api.Playlist_RemoveAt(playlist.Path, index);
        }

        public SuccessResponse CreateNewPlaylist(string name, string[] list)
        {
            var playlist = new Playlist
            {
                Name = name,
                Path = _api.Playlist_CreatePlaylist(String.Empty, name, list),
                Tracks = list.Count()
            };
            using (var db = _mHelper.GetDbConnection())
            {
                db.Save(playlist);
                return new SuccessResponse
                {
                    success = db.GetLastInsertId() > 0
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
            using (var db = _mHelper.GetDbConnection())
            {
                db.DeleteById<Playlist>(id);
                return _api.Playlist_DeletePlaylist(playlist.Path);
            }
        }

        private Playlist GetPlaylistById(int id)
        {
            using (var db = _mHelper.GetDbConnection())
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