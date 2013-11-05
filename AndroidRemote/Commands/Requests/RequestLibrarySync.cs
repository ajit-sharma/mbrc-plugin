using ServiceStack.Text;

namespace MusicBeePlugin.AndroidRemote.Commands.Requests
{
    using Interfaces;
    class RequestLibrarySync : ICommand
    {
        public void Dispose()
        {
            
        }

        public void Execute(IEvent eEvent)
        {
            JsonObject obj = (JsonObject) eEvent.Data;
            string syncType = obj.Get("type");
            string file;
            switch (syncType)
            {
                case "full":
                    Plugin.Instance.SyncGetFilenames();
                    break;
                case "partial":
                    //todo: fix partial sync call
                    break;
                case "cover":
                    file = obj.Get("path");
                    bool hash = obj.Get<bool>("hash");
                    Plugin.Instance.SyncGetCover(file, hash);
                    break;
                case "meta":
                    file = obj.Get("path");
                    Plugin.Instance.SyncGetMetaData(file);
                    break;
            }
            
        }
    }
}
