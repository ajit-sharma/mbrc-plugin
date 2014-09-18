using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    public class LibraryAlbum
    {
        public LibraryAlbum(string name)
        {
            Name = name;
            TrackList = new List<LibraryTrack>();
        }

        public LibraryAlbum()
        {
            TrackList = new List<LibraryTrack>();
        }

        [DataMember(Name = "id")]
        [AutoIncrement]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [References(typeof (LibraryArtist))]
        [DataMember(Name = "artistId")]
        public int ArtistId { get; set; }

        [DataMember(Name = "coverHash")]
        public string CoverHash { get; set; }

        [DataMember(Name = "albumId")]
        public string AlbumId { get; set; }

        public List<LibraryTrack> TrackList { get; set; }
    }
}