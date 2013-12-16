namespace MusicBeePlugin.AndroidRemote.Entities
{
    class LibraryData
    {
        private string hash;
        private string artist;
        private string artist_image_url;
        private string album_artist;
        private string album;
        private string title;
        private string genre;
        private string year;
        private int track_no;
        private string cover_hash;

        public LibraryData(string hash, string artist, string artistImageUrl, string albumArtist, string album, string title, string genre, string year, int trackNo, string coverHash)
        {
            this.hash = hash;
            this.artist = artist;
            this.artist_image_url = artistImageUrl;
            this.album_artist = albumArtist;
            this.album = album;
            this.title = title;
            this.genre = genre;
            this.year = year;
            this.track_no = trackNo;
            this.cover_hash = coverHash;
        }

        public LibraryData()
        {
            this.hash = "";
            this.artist = "";
            this.artist_image_url = "";
            this.album_artist = "";
            this.album = "";
            this.title = "";
            this.genre = "";
            this.year = "";
            this.track_no = 0;
            this.cover_hash = "";
        }

        public string Hash
        {
            get { return hash; }
            set { hash = value; }
        }

        public string Artist
        {
            get { return artist; }
            set { artist = value; }
        }

        public string ArtistImageUrl
        {
            get { return artist_image_url; }
            set { artist_image_url = value; }
        }

        public string AlbumArtist
        {
            get { return album_artist; }
            set { album_artist = value; }
        }

        public string Album
        {
            get { return album; }
            set { album = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Genre
        {
            get { return genre; }
            set { genre = value; }
        }

        public string Year
        {
            get { return year; }
            set { year = value; }
        }

        public int TrackNo
        {
            get { return track_no; }
            set { track_no = value; }
        }

        public string CoverHash
        {
            get { return cover_hash; }
            set { cover_hash = value; }
        }
    }
}
