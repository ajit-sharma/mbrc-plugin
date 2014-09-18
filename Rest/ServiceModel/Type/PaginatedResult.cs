using System.Collections;
using System.Runtime.Serialization;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    public class PaginatedResult
    {
        [DataMember(Name = "total")]
        public int Total { get; set; }
        [DataMember(Name = "limit")]
        public int Limit { get; set; }
        [DataMember(Name = "offset")]
        public int Offset { get; set; }

        [DataMember(Name = "data")]
        public IList Data { get; set; }
    }
}
