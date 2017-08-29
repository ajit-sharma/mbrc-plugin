using System.Collections.Generic;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;

namespace MusicBeeRemote.Core.ApiAdapters
{
    public interface INowPlayingApiAdapter
    {
        IEnumerable<NowPlaying> GetTracks(int offset = 0, int limit = 5000);
        
        /// <summary>
        /// Moves a track in the now Playing list from the initial position to the destination position.
        /// </summary>
        /// <param name="from">The original position of the track</param>
        /// <param name="to">The destination position of the track</param>
        /// <returns>True if the move operation was successful and false if not.</returns>
        bool MoveTrack(int from, int to);
      
        bool RemoveIndex(int index);
        
        bool PlayIndex(int index);

        bool PlayPath(string path);

    }
}