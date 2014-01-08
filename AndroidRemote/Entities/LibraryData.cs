namespace MusicBeePlugin.AndroidRemote.Entities
{
    class LibraryData
    {
        private string hash;
        private string cover_hash;
        private string filepath;

        public LibraryData(string hash, string filepath, string coverhash="")
        {
            this.hash = hash;
            this.cover_hash = coverhash;
            this.Filepath = filepath;
        }

        public LibraryData()
        {
            this.Filepath = Filepath;
            this.hash = "";
            this.cover_hash = "";
            
        }

        public string Hash
        {
            get { return hash; }
            set { hash = value; }
        }

        public string CoverHash
        {
            get { return cover_hash; }
            set { cover_hash = value; }
        }

        public string Filepath
        {
            get { return filepath; }
            set { filepath = value; }
        }
    }
}
