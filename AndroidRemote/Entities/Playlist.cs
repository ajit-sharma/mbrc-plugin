using System.Runtime.Serialization;

namespace MusicBeePlugin.AndroidRemote.Entities
{
    class Playlist
    {
        public string name { get; set; }
        public string hash { get; set; }
        public int tracks { get; set; }
        public bool editable { get; private set; }

        [IgnoreDataMember]
        public string path { get; set; }

        public Playlist(string name, int tracks, string hash, string path)
        {
            this.name = name;
            this.hash = hash;
            this.tracks = tracks;
            this.path = path;

            if (name.Contains("Recently Added") ||
                name.Contains("Recently Played") ||
                name.Contains("Top 25 Most Played") ||
                name.Contains("Top Rated"))
            {
                editable = false;
            }
            else
            {
                editable = true;
            }
        }

        public Playlist()
        {
        }
    }
}
