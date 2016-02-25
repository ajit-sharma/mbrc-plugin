using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeeRemoteCore.ApiAdapters
{
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    class NowPlayingApiAdapter : INowPlayingApiAdapter
    {
        private readonly Plugin.MusicBeeApiInterface api;

        public NowPlayingApiAdapter(Plugin.MusicBeeApiInterface api)
        {
            this.api = api;
        }

        public ICollection<NowPlaying> GetNowPlayingList()
        {
            this.api.NowPlayingList_QueryFiles(null);

            var tracks = new List<NowPlaying>();
            var position = 1;

            while (true)
            {
                var playListTrack = this.api.NowPlayingList_QueryGetNextFile();
                if (string.IsNullOrEmpty(playListTrack))
                {
                    break;
                }

                var artist = this.api.Library_GetFileTag(playListTrack, Plugin.MetaDataType.Artist);
                var title = this.api.Library_GetFileTag(playListTrack, Plugin.MetaDataType.TrackTitle);

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

                tracks.Add(nowPlaying);
                position++;
            }

            return tracks;
        }

        public bool NowPlayingMoveTrack(int @from, int to)
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

            return this.api.NowPlayingList_MoveFiles(aFrom, dIn);
        }

        public bool NowPlayingRemove(int index)
        {
            return this.api.NowPlayingList_RemoveAt(index);
        }

        public bool PlayNow(string path)
        {
            return this.api.NowPlayingList_PlayNow(path);
        }

        public bool QueueLast(string[] tracklist)
        {
            return this.api.NowPlayingList_QueueFilesLast(tracklist);
        }

        public bool QueueNext(string[] tracklist)
        {
            return this.api.NowPlayingList_QueueFilesNext(tracklist);
        }

        public bool QueueNow(string[] tracklist)
        {
            this.api.NowPlayingList_Clear();
            var success = this.api.NowPlayingList_QueueFilesNext(tracklist);

            if (tracklist != null && tracklist.Length > 0)
            {
                this.api.NowPlayingList_PlayNow(tracklist[0]);
            }

            return success;
        }
    }
}
