using System;
using MusicBeePlugin.AndroidRemote.Enumerations;
using MusicBeePlugin.AndroidRemote.Interfaces;
using ServiceStack.Text;

namespace MusicBeePlugin.AndroidRemote.Commands.Requests
{
    class RequestPlaylists : ICommand
    {
        public void Dispose()
        {
        }

        public void Execute(IEvent eEvent)
        {
            var obj = (JsonObject)eEvent.Data;
            var commandType = obj.Get("type");

            int offset;
            int limit;
            switch (commandType)
            {
                case "get":
                    offset = obj.Get<int>("offset");
                    limit = obj.Get<int>("limit");
                    if (limit == 0)
                    {
                        limit = 50;
                    }
                    Plugin.Instance.PlaylistModule.GetAvailablePlaylists(eEvent.ClientId, limit, offset);
                    break;
                case "gettracks":
                    offset = obj.Get<int>("offset");
                    limit = obj.Get<int>("limit");
                    if (limit == 0)
                    {
                        limit = 50;
                    }
                    Plugin.Instance.PlaylistModule.GetTracksForPlaylist(obj.Get("playlist_hash"), eEvent.ClientId, limit, offset);
                    break;
                case "play":
                    var hash = obj.Get("hash");
                    Plugin.Instance.PlaylistModule.RequestPlaylistPlayNow(hash);
                    break;
                case "move":
                    MoveTracks(eEvent);
                    break;
                case "add":
                    AddToPlaylist(eEvent);
                    break;
                case "remove":
                    RemoveTrack(eEvent);
                    break;
                case "create":
                    CreatePlaylist(eEvent);
                    break;
            }

        }

        private static void RemoveTrack(IEvent eEvent)
        {
            var obj = (JsonObject) eEvent.Data;
            var src = obj.Get("src");
            var index = int.Parse(obj.Get("index"));
            Plugin.Instance.PlaylistModule.RequestPlaylistTrackRemove(src, index);
        }

        private static void MoveTracks(IEvent eEvent)
        {
            var obj = (JsonObject) eEvent.Data;
            var src = obj.Get("src");
            var @from = int.Parse(obj.Get("from"));
            var to = int.Parse(obj.Get("to"));
            Plugin.Instance.PlaylistModule.RequestPlaylistMove(eEvent.ClientId, src, @from, to);
        }

        private static void CreatePlaylist(IEvent eEvent)
        {
            var obj = (JsonObject) eEvent.Data;
            var name = obj.Get("name");
            var data = obj.Get("data");
            var selection = obj.Get("selection");
            var tag = (MetaTag) Enum.Parse(typeof (MetaTag), selection);
            Plugin.Instance.PlaylistModule.RequestPlaylistCreate(eEvent.ClientId, name, tag, data);
        }

        private static void AddToPlaylist(IEvent eEvent)
        {
            var obj = (JsonObject) eEvent.Data;
            var hash = obj.Get("hash");
            var data = obj.Get("data");
            var selection = obj.Get("selection");
            var tag = (MetaTag) Enum.Parse(typeof (MetaTag), selection);
            Plugin.Instance.PlaylistModule.RequestPlaylistAddTracks(eEvent.ClientId, hash, tag, data);
        }
    }
}
