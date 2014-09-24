#region

using System;
using System.Collections.Generic;
using MusicBeePlugin.Rest.ServiceModel.Type;
using Ninject;

#endregion

namespace MusicBeePlugin.Modules
{
    public class NowPlayingModule
    {
        private Plugin.MusicBeeApiInterface _api;

        public NowPlayingModule(Plugin.MusicBeeApiInterface api)
        {
            _api = api;
        }

        public bool NowplayingPlayNow(string path)
        {
            return _api.NowPlayingList_PlayNow(path);
        }


        public List<NowPlaying> GetCurrentQueue(string clientId, int offset = 0, int limit = 50)
        {
            _api.NowPlayingList_QueryFiles(null);

            var trackList = new List<NowPlaying>();
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

                var nowPlaying = new NowPlaying
                {
                    Artist = artist,
                    Id = position,
                    Path = playListTrack,
                    Index = position,
                    Title = title
                };

                trackList.Add(nowPlaying);
                position++;
            }

//            var message = new
//            {
//                type = "list",
//                total = trackList.Count,
//                limit,
//                offset,
//                data = trackList.GetRange(offset, limit)
//            };
            //SendSocketMessage(Constants.Nowplaying, Constants.Reply, message, clientId);
            return trackList;
        }

        /// <summary>
        ///     Searches in the Now playing list for the index specified and plays it.
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

            // SendSocketMessage(Constants.NowPlayingListPlay, Constants.Reply, result);
        }

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        public bool CurrentQueueRemoveTrack(int index)
        {
            if (index > 0)
            {
                index--;
            }
            return _api.NowPlayingList_RemoveAt(index);
        }

        /// <summary>
        ///     Moves a index of the now playing list to a new position.
        /// </summary>
        /// <param name="from">The initial position</param>
        /// <param name="to">The final position</param>
        public bool CurrentQueueMoveTrack(int @from, int to)
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

            return _api.NowPlayingList_MoveFiles(aFrom, dIn);
        }
    }
}