namespace plugin_tester
{
    using System;
    using System.Collections.Generic;

    using MusicBeeRemoteCore;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    internal class NowPlayingApiAdapter : INowPlayingApiAdapter
    {
        public ICollection<NowPlaying> GetNowPlayingList()
        {
            throw new NotImplementedException();
        }

        public bool NowPlayingMoveTrack(int @from, int to)
        {
            throw new NotImplementedException();
        }

        public bool NowPlayingRemove(int index)
        {
            throw new NotImplementedException();
        }

        public bool PlayNow(string path)
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