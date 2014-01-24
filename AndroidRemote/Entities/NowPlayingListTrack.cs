namespace MusicBeePlugin.AndroidRemote.Entities
{
    class NowPlayingListTrack
    {
        private readonly int _position;
        private readonly string _artist;
        private readonly string _title;
        private readonly string _hash;

        public NowPlayingListTrack(string artist, string title, int position, string hash)
        {
            _position = position;
            _hash = hash;
            _artist = artist;
            _title = title;
        }

        public string artist
        {
            get { return _artist; }
        }

        public string title
        {
            get { return _title; }
        }

        public int position
        {
            get { return _position; }
        }

        public string hash
        {
            get { return _hash; }
        }
    }
}
