using System.Runtime.Serialization;

namespace MusicBeePlugin.AndroidRemote.Entities
{
    class Playlist
    {
        public string name { get; set; }
        public string hash { get; set; }
        public int tracks { get; set; }

        [IgnoreDataMember]
        public string path { get; set; }

        public Playlist(string name, string hash)
        {
            this.name = name;
            this.hash = hash;
            this.tracks = 0;
        }

        public Playlist(string name, int tracks, string hash, string path)
        {
            this.name = name;
            this.hash = hash;
            this.tracks = tracks;
            this.path = path;
        }

        public Playlist()
        {
        }
    }
}
