using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    public class PositionRequestBody
    {
        [DataMember(Name = "position")]
        public int Position { get; set; }
    }

}