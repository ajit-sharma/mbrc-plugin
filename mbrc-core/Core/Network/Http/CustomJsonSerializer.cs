using Newtonsoft.Json;

namespace MusicBeeRemote.Core.Network.Http
{
    public class CustomJsonSerializer : JsonSerializer
    {
        public CustomJsonSerializer()
        {
            Formatting = Formatting.None;
            NullValueHandling = NullValueHandling.Ignore;
        }
    }
}