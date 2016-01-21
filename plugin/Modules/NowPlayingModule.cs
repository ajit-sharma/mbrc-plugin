#region

using MusicBeePlugin.Rest.ServiceModel.Type;
using System;
using System.Collections.Generic;
using MusicBeePlugin.AndroidRemote.Enumerations;

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


        public PaginatedResponse<NowPlaying> GetCurrentQueue(int offset = 0, int limit = 50)
        {
            _api.NowPlayingList_QueryFiles(null);

            var trackList = new List<NowPlaying>();
            var position = 1;

            while (true)
            {
                var playListTrack = _api.NowPlayingList_QueryGetNextFile();
                if (string.IsNullOrEmpty(playListTrack))
                {
                    break;
                }

                var artist = _api.Library_GetFileTag(playListTrack, Plugin.MetaDataType.Artist);
                var title = _api.Library_GetFileTag(playListTrack, Plugin.MetaDataType.TrackTitle);

                if (string.IsNullOrEmpty(artist))
                {
                    artist = "Unknown Artist";
                }

                if (string.IsNullOrEmpty(title))
                {
                    var index = playListTrack.LastIndexOf('\\');
                    title = playListTrack.Substring(index + 1);
                }

                var nowPlaying = new NowPlaying
                {
                    Artist = artist,
                    Id = position,
                    Path = playListTrack,
                    Position = position,
                    Title = title
                };

                trackList.Add(nowPlaying);
                position++;
            }

            var paginated = new PaginatedNowPlayingResponse();
            paginated.CreatePage(limit, offset, trackList);
            return paginated;
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
            return _api.NowPlayingList_RemoveAt(index);
        }

        /// <summary>
        ///     Moves a index of the now playing list to a new position.
        /// </summary>
        /// <param name="from">The initial position</param>
        /// <param name="to">The final position</param>
        public bool CurrentQueueMoveTrack(int from, int to)
        {
            int[] aFrom = { @from };
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

        /// <summary>
        /// Takes a tracklist and queues it for play depending on the selection 
        /// <paramref name="type"/>
        /// </summary>
        /// <param name="type"><see cref="QueueType"/></param>
        /// <param name="tracklist">An array with the full paths of the files to add to the Queue</param>
        /// <returns></returns>
        public bool EnqueueTracks(QueueType type, string[] tracklist)
        {
            var success = false;
            switch (type)
            {
                case QueueType.now:
                    _api.NowPlayingList_Clear();
                    success = _api.NowPlayingList_QueueFilesNext(tracklist);
                    if (tracklist != null && tracklist.Length > 0)
                    {
                        NowplayingPlayNow(tracklist[0]);    
                    }
                    break;
                case QueueType.last:
                    success = _api.NowPlayingList_QueueFilesLast(tracklist);
                    break;
                case QueueType.next:
                    success = _api.NowPlayingList_QueueFilesNext(tracklist);
                    break;
            }

            return success;
        }
    }
}