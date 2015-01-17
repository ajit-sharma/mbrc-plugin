#region

using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    [Alias("LibraryAlbum")]
    public class LibraryAlbum
    {
        public LibraryAlbum()
        {
            TrackList = new List<LibraryTrack>();
        }

        [AutoIncrement]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [References(typeof (LibraryArtist))]
        [DataMember(Name = "artistId")]
        public int ArtistId { get; set; }

        [DataMember(Name = "coverId")]
        public int CoverId { get; set; }

        [DataMember(Name = "albumId")]
        public string AlbumId { get; set; }

        [Ignore]
        [IgnoreDataMember]
        public List<LibraryTrack> TrackList { get; set; }
    }
}