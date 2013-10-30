namespace MusicBeePlugin.AndroidRemote.Commands.Requests
{
    using Interfaces;
    using ServiceStack.Text;
    class RequestPlaylistMove : ICommand
    {
        public void Dispose()
        {
            
        }

        public void Execute(IEvent eEvent)
        {
            string src;
            int from, to;
            JsonObject obj = (JsonObject) eEvent.Data;
            src = obj.Get("src");
            from = int.Parse(obj.Get("from"));
            to = int.Parse(obj.Get("to"));
            Plugin.Instance.RequestPlaylistMove(eEvent.ClientId, src, from, to);
        }
    }
}
