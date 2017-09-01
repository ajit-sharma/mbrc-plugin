using System;
using System.Collections.Generic;
using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;

namespace MusicBeeRemoteTester.Adapters
{
    internal class NowPlayingApiAdapter : INowPlayingApiAdapter
    {
        public IEnumerable<NowPlaying> GetTracks(int offset = 0, int limit = 5000)
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

        public bool PlayIndex(int index)
        {
            throw new NotImplementedException();
        }

        public bool PlayPath(string path)
        {
            throw new NotImplementedException();
        }
    }
}