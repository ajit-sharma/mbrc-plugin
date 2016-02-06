#region Dependencies

using System.Runtime.Serialization;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
    [DataContract]
    public class SetTrackRating
    {
        [DataMember(Name = "rating", IsRequired = true)]
        public float? Rating { get; set; }
    }

    [DataContract]
    public class SetTrackPosition
    {
        [DataMember(Name = "position", IsRequired = true)]
        public int Position { get; set; }
    }

    [DataContract]
    public class PutLfmRating
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
    }
}