using TinyMessenger;

namespace MusicBeeRemote.Core.Events
{

    internal class LyricsAvailable : ITinyMessage
    {
        public string Lyrics { get; }

        public LyricsAvailable(string lyrics)
        {
            Lyrics = lyrics;
        }

        public object Sender { get; } = null;
    }

    internal class CoverAvailable : ITinyMessage
    {
        public string Cover { get; }

        public CoverAvailable(string cover)
        {
            Cover = cover;
        }

        public object Sender { get; } = null;
    }
}