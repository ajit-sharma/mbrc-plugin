using System;
using System.Collections.Generic;
using MusicBeePlugin.AndroidRemote.Data;
using MusicBeePlugin.AndroidRemote.Entities;
using MusicBeePlugin.AndroidRemote.Enumerations;
using MusicBeePlugin.AndroidRemote.Networking;
using MusicBeePlugin.AndroidRemote.Utilities;

namespace MusicBeePlugin
{
    public class NowPlayingModule : Messenger
    {
        private Plugin.MusicBeeApiInterface _api;
        private CacheHelper mHelper;

        public NowPlayingModule(Plugin.MusicBeeApiInterface api, string storagePath)
        {
            _api = api;
            mHelper = new CacheHelper(storagePath);
        }

        
        private void BuildCurrentQueue()
        {
            _api.NowPlayingList_QueryFiles(null);

            var trackList = new List<NowPlayingListTrack>();
            var position = 1;

            while (true)
            {
                var playListTrack = _api.NowPlayingList_QueryGetNextFile();
                if (String.IsNullOrEmpty(playListTrack))
                {
                    break;
                }

                var artist = _api.Library_GetFileTag(playListTrack, Plugin.MetaDataType.Artist);
                var title = _api.Library_GetFileTag(playListTrack, Plugin.MetaDataType.TrackTitle);

                if (String.IsNullOrEmpty(artist))
                {
                    artist = "Unknown Artist";
                }

                if (String.IsNullOrEmpty(title))
                {
                    var index = playListTrack.LastIndexOf('\\');
                    title = playListTrack.Substring(index + 1);
                }

                trackList.Add(new NowPlayingListTrack(artist, title, position, Utilities.Sha1Hash(playListTrack)));
                position++;
            }

            mHelper.CacheNowPlayingTracks(trackList);
        }

        public void GetCurrentQueue(string clientId, int offset = 0, int limit = 50)
        {
            if (offset == 0)
            {
                BuildCurrentQueue();
            }

            var message = new
            {
                type = "list",
                total = mHelper.GetCurrentQueueTotalTracks(),
                limit,
                offset,
                data = mHelper.GetNowPlayingListTracks(offset, limit)
            };
            SendSocketMessage(Constants.Nowplaying, Constants.Reply, message, clientId);

        }

        /// <summary>
        /// Searches in the Now playing list for the index specified and plays it.
        /// </summary>
        /// <param name="index">The index to play</param>
        /// <returns></returns>
        public void CurrentQueuePlay(string index)
        {
            var result = false;
            int trackIndex;
            if (Int32.TryParse(index, out trackIndex))
            {
                _api.NowPlayingList_QueryFiles(null);
                var trackToPlay = String.Empty;
                var lTrackIndex = 0;
                while (lTrackIndex != trackIndex)
                {
                    trackToPlay = _api.NowPlayingList_QueryGetNextFile();
                    lTrackIndex++;
                }
                if (!String.IsNullOrEmpty(trackToPlay))
                    result = _api.NowPlayingList_PlayNow(trackToPlay);
            }

            SendSocketMessage(Constants.NowPlayingListPlay, Constants.Reply, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="clientId"></param>
        public void CurrentQueueRemoveTrack(int index, string clientId)
        {
            var reply = new
            {
                success = _api.NowPlayingList_RemoveAt(index),
                index
            };
            SendSocketMessage(Constants.NowPlayingListRemove, Constants.Reply,reply, clientId);
        }

        /// <summary>
        /// Moves a index of the now playing list to a new position.
        /// </summary>
        /// <param name="clientId">The Id of the client that initiated the request</param>
        /// <param name="from">The initial position</param>
        /// <param name="to">The final position</param>
        public void CurrentQueueMoveTrack(string clientId, int from, int to)
        {
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

            var result = _api.NowPlayingList_MoveFiles(aFrom, dIn);

            var reply = new
            {
                success = result, @from, to
            };

            SendSocketMessage(Constants.NowPlayingListMove, Constants.Reply, reply, clientId);
        }

        public void NowPlayingQueueTracks(MetaTag tag, string query, QueueType type)
        {
            var tracks = Plugin.Instance.GetUrlsForTag(tag, query);

            switch (type)
            {
                case QueueType.last:
                    _api.NowPlayingList_QueueFilesLast(tracks);
                    break;
                case QueueType.next:
                    _api.NowPlayingList_QueueFilesNext(tracks);
                    break;
                case QueueType.now:
                    _api.NowPlayingList_Clear();
                    _api.NowPlayingList_QueueFilesNext(tracks);
                    _api.NowPlayingList_PlayNow(tracks[0]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}