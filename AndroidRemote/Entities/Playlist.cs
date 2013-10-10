namespace MusicBeePlugin.AndroidRemote.Entities
{
    class Playlist
    {
        public string name { get; set; }
        public string src { get; set; }
        public int count { get; set; }

        public Playlist(string name, string src)
        {
            this.name = name;
            this.src = src;
            this.count = 0;
        }
    }
}
