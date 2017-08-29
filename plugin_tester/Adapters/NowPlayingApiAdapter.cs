using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;

namespace plugin_tester
{
    using System;
    using System.Collections.Generic;

    internal class NowPlayingApiAdapter : INowPlayingApiAdapter
    {
        public ICollection<NowPlaying> GetTracks()
        {
            throw new NotImplementedException();
        }

        public bool MoveTrack(int @from, int to)
        {
            throw new NotImplementedException();
        }

        public bool RemoveIndex(int index)
        {
            throw new NotImplementedException();
        }

        public bool PlayPath(string path)
        {
            throw new NotImplementedException();
        }

        public bool QueueLast(string[] tracklist)
        {
            throw new NotImplementedException();
        }

        public bool QueueNext(string[] tracklist)
        {
            throw new NotImplementedException();
        }

        public bool QueueNow(string[] tracklist)
        {
            throw new NotImplementedException();
        }
    }
}