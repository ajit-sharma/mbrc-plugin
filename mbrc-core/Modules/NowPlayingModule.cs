namespace MusicBeeRemoteCore.Modules
{
    using System.Linq;

    using MusicBeeRemoteCore.AndroidRemote.Enumerations;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    public class NowPlayingModule
    {
        private INowPlayingApiAdapter api;

        public NowPlayingModule(INowPlayingApiAdapter api)
        {
            this.api = api;
        }

        /// <summary>
        ///     Moves a index of the now playing list to a new position.
        /// </summary>
        /// <param name="from">The initial position</param>
        /// <param name="to">The final position</param>
        public bool CurrentQueueMoveTrack(int from, int to)
        {
            return this.api.NowPlayingMoveTrack(from, to);
        }

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        public bool CurrentQueueRemoveTrack(int index)
        {
            return this.api.NowPlayingRemove(index);
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
                    success = this.api.QueueNow(tracklist);
                    break;
                case QueueType.last:
                    success = this.api.QueueLast(tracklist);
                    break;
                case QueueType.next:
                    success = this.api.QueueNext(tracklist);
                    break;
            }

            return success;
        }

        public PaginatedResponse<NowPlaying> GetCurrentQueue(int offset = 0, int limit = 50)
        {
            var tracks = this.api.GetNowPlayingList();
            var paginated = new PaginatedNowPlayingResponse();
            paginated.CreatePage(limit, offset, tracks.ToList());
            return paginated;
        }

        public bool NowplayingPlayNow(string path)
        {
            return this.api.PlayNow(path);
        }
    }
}