using System;
using MusicBeePlugin.Rest.ServiceModel.Const;
using MusicBeePlugin.Rest.ServiceModel.Enum;
using ServiceStack.Api.Swagger;
using ServiceStack.ServiceHost;

namespace MusicBeePlugin.Rest.ServiceModel.Requests
{
	/// <summary>
	/// Base of all the sync requests.
	/// </summary>
	public abstract class SyncRequest : PaginatedRequest
	{
		[ApiMember(Name = "change", ParameterType = "query", DataType = SwaggerType.String, IsRequired = true, Description = Descriptions.UpdateChange)]
		[ApiAllowableValues("change", typeof(ChangeType))]
		public ChangeType Change { get; set; }

		[ApiMember(Name = "lastSync", ParameterType = "query", DataType = SwaggerType.String, IsRequired = true, Description = Descriptions.TheDateOfTheLastSync)]
		public DateTime LastSync { get; set; }
	}
}