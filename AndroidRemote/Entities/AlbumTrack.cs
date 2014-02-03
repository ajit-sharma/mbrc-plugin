using System;

namespace MusicBeePlugin.AndroidRemote.Entities
{
    class AlbumTrack : IComparable<AlbumTrack>
    {
        private int _track_no;
        private string _path;

        public AlbumTrack(string path, int trackNo) 
        {
            Path = path;
            TrackNo = trackNo;
        }

        public int TrackNo
        {
            get { return _track_no; }
            set { _track_no = value; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }


        public int CompareTo(AlbumTrack other)
        {
            var trackNo = other.TrackNo;
            return trackNo == _track_no ? 0 : trackNo > _track_no ? -1 : 1;
        }
    }
}
