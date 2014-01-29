using System;
using System.Diagnostics;
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
            try
            {
                JsonObject obj = (JsonObject)eEvent.Data;
                string syncType = obj.Get("type");
                string file;
                switch (syncType)
                {
                    case "full":
                        Plugin.Instance.SyncModule.SyncGetFilenames(eEvent.ClientId);
                        break;
                    case "partial":

                        string[] cachedFiles = obj.Get<string[]>("files");
                        string lastSync = obj.Get("lastsync");
                        //todo: fix partial sync call
                        break;
                    case "cover":
                        file = obj.Get("hash");
                        Plugin.Instance.SyncModule.SyncGetCover(file, eEvent.ClientId);
                        break;
                    case "meta":
                        int offset = obj.Get<int>("offset");
                        int limit = obj.Get<int>("limit");
                        Plugin.Instance.SyncModule.SyncGetMetaData(offset, eEvent.ClientId, limit);
                        break;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("Type: " +eEvent.Data.GetType() + "\t data: " + eEvent.Data);
                if (eEvent.Data.GetType() == typeof (JsonObject))
                {
                    Debug.WriteLine(((JsonObject)eEvent.Data).Dump());
                }
                Debug.WriteLine(ex);
#endif
            }
        }
    }
}
