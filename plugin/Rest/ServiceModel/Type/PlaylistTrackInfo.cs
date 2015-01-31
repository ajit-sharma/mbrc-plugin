#region

using System;
using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    public class PlaylistTrackInfo : IEquatable<PlaylistTrackInfo>
    {
        [AutoIncrement]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "path")]
        public string Path { get; set; }

        [DataMember(Name = "artist")]
        public string Artist { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

	    public bool Equals(PlaylistTrackInfo other)
	    {
		    return Path.Equals(other.Path);
	    }
    }
}