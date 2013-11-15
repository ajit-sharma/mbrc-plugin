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
                        Plugin.Instance.SyncGetFilenames();
                        break;
                    case "partial":

                        string[] cachedFiles = obj.Get<string[]>("files");
                        string lastSync = obj.Get("lastsync");
                        //todo: fix partial sync call
                        break;
                    case "cover":
                        file = obj.Get("file");
                        bool hash = obj.Get<bool>("hash");
                        Plugin.Instance.SyncGetCover(file, hash);
                        break;
                    case "meta":
                        file = obj.Get("file");
                        Plugin.Instance.SyncGetMetaData(file);
                        break;
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Type: " +eEvent.Data.GetType() + "\t data: " + eEvent.Data);
            }
        }
    }
}
