using System.Runtime.Serialization;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    internal class Playlist
    {
        public Playlist(string name, int tracks, string path)
        {
            Name = name;
            Tracks = tracks;
            Path = path;

            if (name.Contains("Recently Added") ||
                name.Contains("Recently Played") ||
                name.Contains("Top 25 Most Played") ||
                name.Contains("Top Rated"))
            {
                ReadOnly = true;
            }
            else
            {
                ReadOnly = false;
            }
        }

        public Playlist()
        {
        }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "tracks")]
        public int Tracks { get; set; }

        [DataMember(Name = "readOnly")]
        public bool ReadOnly { get; private set; }

        [DataMember(Name = "path")]
        public string Path { get; set; }
    }
}