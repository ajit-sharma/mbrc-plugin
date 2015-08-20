using MusicBeePlugin.Rest.ServiceModel.Const;
using ServiceStack.Api.Swagger;
using ServiceStack.ServiceHost;

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
	}
}