using System;
using System.Collections.Generic;

namespace MusicBeePlugin.AndroidRemote.Entities
{
    class AlbumEntry : IEquatable<AlbumEntry>
    {
        private String _album_id;
        private List<AlbumTrack> _tracklist;

        public AlbumEntry(string albumId)
        {
            AlbumId = albumId;
            Tracklist = new List<AlbumTrack>();
        }

        public List<AlbumTrack> Tracklist
        {
            get { return _tracklist; }
            set { _tracklist = value; }
        }

        public string AlbumId
        {
            get { return _album_id; }
            set { _album_id = value; }
        }

        public bool Equals(AlbumEntry other)
        {
            return other.AlbumId.Equals(_album_id);
        }
    }
}
