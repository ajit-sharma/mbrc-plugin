using Newtonsoft.Json;

namespace MusicBeeRemote.Core.Rest
{
    public class CustomJsonSerializer : JsonSerializer
    {
        public CustomJsonSerializer()
        {
            this.Formatting = Formatting.None;
            this.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}