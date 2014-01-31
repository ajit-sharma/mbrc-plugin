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

            switch (commandType)
            {
                case "get":
                    Plugin.Instance.GetAvailablePlaylists();
                    break;
                case "getfiles":
                    Plugin.Instance.GetTracksForPlaylist(eEvent.DataToString(), eEvent.ClientId);
                    break;
                case "play":
                    Plugin.Instance.RequestPlaylistPlayNow(eEvent.DataToString());
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

            Plugin.Instance.RequestPlaylistTrackRemove(src, index);
        }

        private void moveTracks(IEvent eEvent)
        {
            string src;
            int from, to;
            JsonObject obj = (JsonObject) eEvent.Data;
            src = obj.Get("src");
            @from = int.Parse(obj.Get("from"));
            to = int.Parse(obj.Get("to"));
            Plugin.Instance.RequestPlaylistMove(eEvent.ClientId, src, @from, to);
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

            Plugin.Instance.RequestPlaylistCreate(eEvent.ClientId, name, tag, query, files);
        }
    }
}
