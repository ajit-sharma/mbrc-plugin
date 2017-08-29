using System.Runtime.Serialization;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;

namespace MusicBeeRemote.Core.Feature.Podcasts
{
    [DataContract]
    public class ArtworkResponse : ResponseBase
    {
        [DataMember(Name = "artwork")]
        public string Artwork { get; set; }
    }
}