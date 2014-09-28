using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    public class PaginatedResponse
    {
        [DataMember(Name = "total")]
        public int Total { get; set; }
        [DataMember(Name = "limit")]
        public int Limit { get; set; }
        [DataMember(Name = "offset")]
        public int Offset { get; set; }

        [DataMember(Name = "data")]
        public IList Data { get; set; }

        public static PaginatedResponse GetPaginatedData<T>(int limit, int offset, List<T> data)
        {
            var result = new PaginatedResponse
            {
                Data = data,
                Offset = offset,
                Limit = limit,
                Total = data.Count
            };

            if (offset == 0 && limit == 0) return result;

            var range = offset + limit;
            var size = data.Count;
            if (range <= size)
            {
                data = data.GetRange(offset, limit);
                result.Data = data;
            }
            else if (offset < size)
            {
                limit = size - offset;
                data = data.GetRange(offset, limit);
                result.Data = data;
                result.Limit = limit;
            }
            return result;
        }
    }
}
