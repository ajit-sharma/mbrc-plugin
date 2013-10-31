using System;
using System.Diagnostics;
using MusicBeePlugin.AndroidRemote.Enumerations;

namespace MusicBeePlugin.AndroidRemote.Commands.Requests
{
    using Interfaces;
    using ServiceStack.Text;
    class RequestPlaylistCreate : ICommand
    {
        public void Dispose()
        {
        }

        public void Execute(IEvent eEvent)
        {
            JsonObject obj = (JsonObject) eEvent.Data;
            string type, name, query;
            string[] files = {};
            type = obj.Get("type");
            name = obj.Get("name");
            query = obj.Get("query");
            files = obj.Get<string[]>("data");
            MetaTag tag = (MetaTag)Enum.Parse(typeof(MetaTag), type);

            Plugin.Instance.RequestPlaylistCreate(eEvent.ClientId, name, tag, query, files);
        }
    }
}
