namespace MusicBeeRemoteCore.Rest.Compression
{
    using System.Collections.Generic;

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