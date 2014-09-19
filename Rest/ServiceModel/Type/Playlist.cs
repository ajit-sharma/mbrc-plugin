#region

using System;
using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    internal class Playlist : IComparable<Playlist>
    {
        private string _name;

        public Playlist(string name, int tracks, string path)
        {
            Name = name;
            Tracks = tracks;
            Path = path;
        }

        public Playlist()
        {
        }

        [AutoIncrement]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (_name.Contains("Recently Added") ||
                    _name.Contains("Recently Played") ||
                    _name.Contains("Top 25 Most Played") ||
                    _name.Contains("Top Rated"))
                {
                    ReadOnly = true;
                }
                else
                {
                    ReadOnly = false;
                }
            }
        }

        [DataMember(Name = "tracks")]
        public int Tracks { get; set; }

        [DataMember(Name = "readOnly")]
        public bool ReadOnly { get; private set; }

        [DataMember(Name = "path")]
        public string Path { get; set; }

        public int CompareTo(Playlist other)
        {
            return String.Compare(Path, other.Path, StringComparison.Ordinal);
        }
    }
}