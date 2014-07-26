

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    public class NowPlaying
    {
        public int id { get; set; }
        public string artist { get; set; }
        public string title { get; set; }
        public int position { get; set; }
        public string path { get; set; }
    }
}
