namespace MusicBeePlugin.AndroidRemote.Commands.Requests
{
    using ServiceStack.Text;
    using Interfaces;
    class RequestPlaylistRemove:ICommand
    {
        public void Dispose()
        {
        }

        public void Execute(IEvent eEvent)
        {
            string src;
            int index;

            JsonObject obj = (JsonObject) eEvent.Data;
            src = obj.Get("src");
            index = int.Parse(obj.Get("index"));

            Plugin.Instance.RequestPlaylistTrackRemove(src,index);
        }
    }
}
