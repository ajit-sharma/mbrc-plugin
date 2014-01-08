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
#if DEBUG
            Debug.WriteLine(DateTime.Now + " Processing Library Request");
#endif
            try
            {
                JsonObject obj = (JsonObject)eEvent.Data;
                string syncType = obj.Get("type");
                string file;
                switch (syncType)
                {
                    case "full":
                        Plugin.Instance.SyncGetFilenames(eEvent.ClientId);
                        break;
                    case "partial":

                        string[] cachedFiles = obj.Get<string[]>("files");
                        string lastSync = obj.Get("lastsync");
                        //todo: fix partial sync call
                        break;
                    case "cover":
                        file = obj.Get("hash");
                        Plugin.Instance.SyncGetCover(file, eEvent.ClientId);
                        break;
                    case "meta":
                        int track = obj.Get<int>("file");
                        Plugin.Instance.SyncGetMetaData(track, eEvent.ClientId);
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
