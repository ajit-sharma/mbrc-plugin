using MusicBeePlugin.Rest.ServiceModel.Const;
using NServiceKit.Api.Swagger;
using NServiceKit.ServiceHost;

namespace MusicBeePlugin.Rest.ServiceModel.Requests
{
	/// <summary>
	/// Base class for the requests of an item by id
	/// </summary>
	public abstract class IdBasedRequest
	{
		[ApiMember(Name = "id", ParameterType = "path", DataType = SwaggerType.Int, IsRequired = true,
			Description = Descriptions.EntryId)]
		public int Id { get; set; }
	}
}