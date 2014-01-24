using System;

namespace MusicBeePlugin.AndroidRemote.Entities
{
    /// <summary>
    /// Class MetaData. 
    /// Represents a packet payload for library meta data.
    /// </summary>
    class MetaData
    {
        private string _file;
        private const string _type = "meta";
        private string _hash;
        private string _artist;
        private string _album_artist;
        private string _album;
        private string _title;
        private string _genre;
        private string _year;
        private string _track_no;
        private string _cover_hash;
        private string _artist_image_url;

        public string file
        {
            get { return _file; }
            set { _file = value; }
        }

        public string album
        {
            get { return _album; }
            set { _album = String.IsNullOrEmpty(value) ? "Unknown Album" : value; }
        }

        public string title
        {
            get { return _title; }
            set {
                _title = !String.IsNullOrEmpty(value)
                    ? value
                    : (String.IsNullOrEmpty(_file) 
                        ? String.Empty 
                        : _file.Substring(_file.LastIndexOf('\\') + 1));
            }
        }

        public string genre
        {
            get { return _genre; }
            set { _genre = String.IsNullOrEmpty(value) ? "Unknown Genre" : value; }
        }

        public string year
        {
            get { return _year; }
            set { _year = value; }
        }

        public string track_no
        {
            get { return _track_no; }
            set { _track_no = value; }
        }

        public string cover_hash
        {
            get { return _cover_hash; }
            set { _cover_hash = value; }
        }

        public string artist_image_url
        {
            get { return _artist_image_url; }
            set { _artist_image_url = value; }
        }

        public string type
        {
            get { return _type; }
        }

        public string hash
        {
            get { return _hash; }
            set { _hash = value; }
        }

        public string artist
        {
            get { return _artist; }
            set { _artist = String.IsNullOrEmpty(value) ? "Unknown Artist" : value; }
        }

        public string album_artist
        {
            get { return _album_artist; }
            set { _album_artist = value; }
        }
    }
}
