using System.IO.Compression;
using System.Linq;
using Nancy;
using Nancy.Bootstrapper;

namespace MusicBeeRemote.Core.Rest.Compression
{
    /// <summary>
    /// Gzip Compression extension from
    /// https://github.com/dcomartin/Nancy.Gzip
    /// </summary>
    public static class GzipCompression
    {
        private static GzipCompressionSettings _settings;

        public static void EnableGzipCompression(this IPipelines pipelines, GzipCompressionSettings settings)
        {
            _settings = settings;
            pipelines.AfterRequest += CheckForCompression;
        }

        public static void EnableGzipCompression(this IPipelines pipelines)
        {
            EnableGzipCompression(pipelines, new GzipCompressionSettings());
        }

        private static void CheckForCompression(NancyContext context)
        {
            if (!RequestIsGzipCompatible(context.Request))
            {
                return;
            }

            if (context.Response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }

            if (!ResponseIsCompatibleMimeType(context.Response))
            {
                return;
            }

            if (ContentLengthIsTooSmall(context.Response))
            {
                return;
            }

            CompressResponse(context.Response);
        }

        private static void CompressResponse(Response response)
        {
            response.Headers["Content-Encoding"] = "gzip";

            var contents = response.Contents;
            response.Contents = responseStream =>
                {
                    using (var compression = new GZipStream(responseStream, CompressionMode.Compress))
                    {
                        contents(compression);
                    }
                };
        }

        private static bool ContentLengthIsTooSmall(Response response)
        {
            string contentLength;
            if (response.Headers.TryGetValue("Content-Length", out contentLength))
            {
                var length = long.Parse(contentLength);
                if (length < _settings.MinimumBytes)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool RequestIsGzipCompatible(Request request)
        {
            return request.Headers.AcceptEncoding.Any(x => x.Contains("gzip"));
        }

        private static bool ResponseIsCompatibleMimeType(Response response)
        {
            return _settings.MimeTypes.Any(x => x == response.ContentType || response.ContentType.StartsWith($"{x};"));
        }
    }
}