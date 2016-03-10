namespace MusicBeeRemoteCore.Rest
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class CustomJsonSerializer : JsonSerializer
    {
        public CustomJsonSerializer()
        {
            this.Formatting = Formatting.None;
            this.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}