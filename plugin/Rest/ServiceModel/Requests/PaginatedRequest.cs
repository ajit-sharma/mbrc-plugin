using MusicBeePlugin.Rest.ServiceModel.Const;
using NServiceKit.Api.Swagger;
using NServiceKit.ServiceHost;

namespace MusicBeePlugin.Rest.ServiceModel.Requests
{
    /// <summary>
    /// The base class for every Paginated request.
    /// </summary>
    public abstract class PaginatedRequest
    {
        [ApiMember(Name = "limit", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = false,
            Description = Descriptions.Limit)]
        public int Limit { get; set; }

        [ApiMember(Name = "offset", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = false,
            Description = Descriptions.Offset)]
        public int Offset { get; set; }

        [ApiMember(Name = "after", ParameterType = "query", DataType = SwaggerType.Long, IsRequired = false,
            Description = Descriptions.TheDateOfTheLastSync)]
        public long After { get; set; }
    }
}