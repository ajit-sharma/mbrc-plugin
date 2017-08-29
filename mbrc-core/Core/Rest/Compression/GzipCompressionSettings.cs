using System.Collections.Generic;

namespace MusicBeeRemote.Core.Rest.Compression
{
    /// <summary>
    /// Gzip Compression settings from
    /// https://github.com/dcomartin/Nancy.Gzip
    /// </summary>
    public class GzipCompressionSettings
    {
        public IList<string> MimeTypes { get; set; } = new List<string>
                                                           {
                                                               "text/plain", 
                                                               "text/html", 
                                                               "text/xml", 
                                                               "text/css", 
                                                               "application/json", 
                                                               "application/x-javascript", 
                                                               "application/atom+xml", 
                                                           };

        public int MinimumBytes { get; set; } = 4096;
    }
}