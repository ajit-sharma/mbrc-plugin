namespace MusicBeePlugin.AndroidRemote
{
    internal class Page
    {
        public Page(string offset, string limit, string after)
        {
            int parsedOffset;
            int parsedLimit;
            int parsedAfter;
            Offset = int.TryParse(offset, out parsedOffset) ? parsedOffset : 0;
            Limit = int.TryParse(limit, out parsedLimit) ? parsedLimit : 0;
            After = int.TryParse(after, out parsedAfter) ? parsedAfter : 0;
        }

        public int Offset { get; private set; }
        public int Limit { get; private set; }
        public int After { get; private set; }
    }
}