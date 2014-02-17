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
            JsonObject obj = (JsonObject)eEvent.Data;
            string commandType = obj.Get("type");

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
                    Plugin.Instance.PlaylistModule.RequestPlaylistPlayNow(eEvent.DataToString());
                    break;
                case "move":
                    moveTracks(eEvent);
                    break;
                case "add":
                    break;
                case "remove":
                    removeTrack(eEvent);
                    break;
                case "create":
                    createPlaylist(eEvent);
                    break;
            }

        }

        private void removeTrack(IEvent eEvent)
        {
            string src;
            int index;

            JsonObject obj = (JsonObject) eEvent.Data;
            src = obj.Get("src");
            index = int.Parse(obj.Get("index"));

            Plugin.Instance.PlaylistModule.RequestPlaylistTrackRemove(src, index);
        }

        private void moveTracks(IEvent eEvent)
        {
            string src;
            int from, to;
            JsonObject obj = (JsonObject) eEvent.Data;
            src = obj.Get("src");
            @from = int.Parse(obj.Get("from"));
            to = int.Parse(obj.Get("to"));
            Plugin.Instance.PlaylistModule.RequestPlaylistMove(eEvent.ClientId, src, @from, to);
        }

        private void createPlaylist(IEvent eEvent)
        {
            JsonObject obj = (JsonObject) eEvent.Data;
            string type, name, query;
            string[] files = {};
            type = obj.Get("type");
            name = obj.Get("name");
            query = obj.Get("query");
            files = obj.Get<string[]>("data");
            MetaTag tag = (MetaTag) Enum.Parse(typeof (MetaTag), type);

            Plugin.Instance.PlaylistModule.RequestPlaylistCreate(eEvent.ClientId, name, tag, query, files);
        }
    }
}
