using Newtonsoft.Json;

namespace MusicBeeRemote.Core.Tools
{
    public class JsonAdapter
    {
        public static string Serialize(object toSerialize)
        {
            return JsonConvert.SerializeObject(toSerialize);
        }
    }
}