using System;
using MusicBeePlugin.AndroidRemote.Enumerations;
using MusicBeePlugin.AndroidRemote.Interfaces;
using NLog;
using ServiceStack.Text;

namespace MusicBeePlugin.AndroidRemote.Commands.Requests
{
    internal class RequestNowPlaying : ICommand
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Dispose()
        {
        }

        public void Execute(IEvent eEvent)
        {
            var obj = (JsonObject) eEvent.Data;
            var commandType = obj.Get("type");

            switch (commandType)
            {
                case "queue":
                    var tag = (MetaTag) Enum.Parse(typeof (MetaTag), obj.Get("selection"));
                    var position = (QueueType) Enum.Parse(typeof (QueueType), obj.Get("position"));
                    var query = obj.Get("data");
                    Plugin.Instance.NowPlayingModule.NowPlayingQueueTracks(tag, query, position);
                    break;
                case "list":
                    var offset = obj.Get<int>("offset");
                    var limit = obj.Get<int>("limit");
                    if (limit == 0)
                    {
                        limit = 50;
                    }
                    Plugin.Instance.NowPlayingModule.GetCurrentQueue(eEvent.ClientId, offset, limit);
                    break;
                case "move":
                    var @from = obj.Get<int>("from");
                    var to = obj.Get<int>("to");
                    Plugin.Instance.NowPlayingModule.CurrentQueueMoveTrack(eEvent.ClientId, from, to);
                    break;
                case "play":
                    //todo: fix get hash and map to path
                    Plugin.Instance.NowPlayingModule.CurrentQueuePlay(eEvent.DataToString());
                    break;
                case "remove":
                    var index = obj.Get<int>("index");
                    Plugin.Instance.NowPlayingModule.CurrentQueueRemoveTrack(index, eEvent.ClientId);
                    break;
                default:
                    Logger.Info("Unknown Action");
                    break;
            }
        }
    }
}