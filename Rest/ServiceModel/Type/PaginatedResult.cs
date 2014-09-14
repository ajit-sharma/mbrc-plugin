using System.Collections;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    public class PaginatedResult
    {
        public int total { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }

        public IList data { get; set; }
    }
}
