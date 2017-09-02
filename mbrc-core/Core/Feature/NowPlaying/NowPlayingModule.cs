using System.Linq;
using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Network.Http.Responses.Type;

namespace MusicBeeRemote.Core.Feature.NowPlaying
{
    public class NowPlayingModule
    {
        private readonly IQueueAdapter _queueAdapter;
        private INowPlayingApiAdapter _nowPlayingAdapter;

        public NowPlayingModule(INowPlayingApiAdapter nowPlayingAdapter, IQueueAdapter queueAdapter)
        {
            _queueAdapter = queueAdapter;
            _nowPlayingAdapter = nowPlayingAdapter;
        }

        /// <summary>
        ///     Moves a index of the now playing list to a new position.
        /// </summary>
        /// <param name="from">The initial position</param>
        /// <param name="to">The final position</param>
        public bool CurrentQueueMoveTrack(int from, int to)
        {
            return _nowPlayingAdapter.MoveTrack(from, to);
        }

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        public bool CurrentQueueRemoveTrack(int index)
        {
            return _nowPlayingAdapter.RemoveIndex(index);
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
            return _queueAdapter.QueueFiles(type, tracklist);
        }

        public PaginatedResponse<Network.Http.Responses.Type.NowPlaying> GetCurrentQueue(int offset = 0, int limit = 50)
        {
            var tracks = _nowPlayingAdapter.GetTracks();
            var paginated = new PaginatedNowPlayingResponse();
            paginated.CreatePage(limit, offset, tracks.ToList());
            return paginated;
        }

        public bool NowplayingPlayNow(string path)
        {
            return _nowPlayingAdapter.PlayPath(path);
        }
    }
}