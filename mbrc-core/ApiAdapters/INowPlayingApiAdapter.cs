namespace MusicBeePlugin
{
    using System.Collections.Generic;

    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface INowPlayingApiAdapter
    {
        ICollection<NowPlaying> GetNowPlayingList();

        bool NowPlayingMoveTrack(int @from, int to);

        bool NowPlayingRemove(int index);

        bool PlayNow(string path);

        bool QueueLast(string[] tracklist);

        bool QueueNext(string[] tracklist);

        bool QueueNow(string[] tracklist);
    }
}