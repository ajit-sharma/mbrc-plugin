using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Text;

namespace MusicBeePlugin.AndroidRemote.Entities
{
    internal class TrackEntry
    {
        private string artist;
        private string albumartist;
        private string album;
        private string year;
        private string genre;
        private string tracknumber;
        private string cover;

        public TrackEntry()
        {

        }

        public string Artist
        {
            get { return artist; }
            set { artist = value; }
        }

        public string Albumartist
        {
            get { return albumartist; }
            set { albumartist = value; }
        }

        public string Album
        {
            get { return album; }
            set { album = value; }
        }

        public string Year
        {
            get { return year; }
            set { year = value; }
        }

        public string Genre
        {
            get { return genre; }
            set { genre = value; }
        }

        public string Tracknumber
        {
            get { return tracknumber; }
            set { tracknumber = value; }
        }

        public string Cover
        {
            get { return cover; }
            set { cover = value; }
        }

        public string Url { get; set; }
        public string Title { get; set; }

}
}
