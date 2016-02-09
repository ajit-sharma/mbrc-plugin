namespace MusicBeePlugin.AndroidRemote
{
    internal class Page
    {
        public Page(string offset, string limit, string after)
        {
            int parsedOffset;
            int parsedLimit;
            int parsedAfter;
            this.Offset = int.TryParse(offset, out parsedOffset) ? parsedOffset : 0;
            this.Limit = int.TryParse(limit, out parsedLimit) ? parsedLimit : 0;
            this.After = int.TryParse(after, out parsedAfter) ? parsedAfter : 0;
        }

        public int After { get; private set; }

        public int Limit { get; private set; }

        public int Offset { get; private set; }
    }
}