using System;
using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    public class LibraryTrack : IComparable<LibraryTrack>
    {
        [AutoIncrement]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "index")]
        public int Index { get; set; }

        [DataMember(Name = "genreId")]
        [References(typeof (LibraryGenre))]
        public int GenreId { get; set; }

        [DataMember(Name = "artistId")]
        [References(typeof (LibraryArtist))]
        public int ArtistId { get; set; }

        [DataMember(Name = "albumArtistId")]
        [References(typeof (LibraryArtist))]
        public int AlbumArtistId { get; set; }

        [DataMember(Name = "albumId")]
        [References(typeof (LibraryAlbum))]
        public int AlbumId { get; set; }

        [DataMember(Name = "year")]
        public string Year { get; set; }

        [DataMember(Name = "path")]
        public string Path { get; set; }

        public int CompareTo(LibraryTrack other)
        {
            var oIndex = other.Index;
            return oIndex == Index ? 0 : oIndex > Index ? -1 : 1;
        }
    }
}