namespace MusicBeeRemoteCore.Rest.StatusCodeHandlers
{
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    using Nancy;
    using Nancy.ErrorHandling;
    using Nancy.Extensions;
    using Nancy.Responses;
    using Nancy.Serialization.JsonNet;

    class JsonStatusHandler : IStatusCodeHandler
    {
        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var errorResponse = new ErrorResponse
                                    {
                                        Code = (int)statusCode,
                                        Message = context.GetException().Message,
                                        Description = context.GetExceptionDetails(),
                                    };

            context.Response = new JsonResponse(errorResponse, new JsonNetSerializer());
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound || statusCode == HttpStatusCode.InternalServerError
                   || statusCode == HttpStatusCode.Forbidden || statusCode == HttpStatusCode.Unauthorized;
        }
    }
}