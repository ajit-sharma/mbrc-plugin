using Newtonsoft.Json;

namespace MusicBeeRemoteCore.Tools
{
    public class JsonAdapter
    {
        public static string Serialize(object toSerialize)
        {
            return JsonConvert.SerializeObject(toSerialize);
        }
    }
}