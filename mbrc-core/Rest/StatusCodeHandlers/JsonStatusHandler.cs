using System.Collections.Generic;

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

        private Dictionary<HttpStatusCode, string> messages = new Dictionary<HttpStatusCode, string>();

        public JsonStatusHandler()
        {
			messages.Add(HttpStatusCode.NotFound, "Not found");
            messages.Add(HttpStatusCode.Forbidden, "Access forbidden");
            messages.Add(HttpStatusCode.Unauthorized, "Unauthorized");
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var exception = context.GetException();
            string defaultMessage;

            if (!messages.TryGetValue(statusCode, out defaultMessage))
            {
                defaultMessage = "There was a problem";
            }

            var message = exception?.Message ?? defaultMessage;
            var errorResponse = new ErrorResponse
            {
                Code = (int) statusCode,
                Message = message,
                Description = context.GetExceptionDetails(),
            };

            context.Response = new JsonResponse(errorResponse, new JsonNetSerializer()) {StatusCode = statusCode};
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound || statusCode == HttpStatusCode.InternalServerError
                   || statusCode == HttpStatusCode.Forbidden || statusCode == HttpStatusCode.Unauthorized;
        }
    }
}