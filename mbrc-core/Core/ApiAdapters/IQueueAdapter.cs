using MusicBeeRemote.Core.Feature.NowPlaying;

namespace MusicBeeRemote.Core.ApiAdapters
{
    public interface IQueueAdapter
    {
        bool QueueFiles(QueueType queue, string[] data, string query = "");
    }
}