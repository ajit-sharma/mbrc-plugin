#region

using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    internal class PlaylistTrack
    {
        [AutoIncrement]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "index")]
        public int Index { get; set; }

        [DataMember(Name = "path")]
        public string Path { get; set; }

        [DataMember(Name = "playlistId")]
        [References(typeof (Playlist))]
        public int PlaylistId { get; set; }

        [DataMember(Name = "artist")]
        public string Artist { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }
    }
}