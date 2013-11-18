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
                        Plugin.Instance.SyncGetFilenames(eEvent.ClientId);
                        break;
                    case "partial":

                        string[] cachedFiles = obj.Get<string[]>("files");
                        string lastSync = obj.Get("lastsync");
                        //todo: fix partial sync call
                        break;
                    case "cover":
                        file = obj.Get("hash");
                        Plugin.Instance.SyncGetCover(file);
                        break;
                    case "meta":
                        int track = obj.Get<int>("file");
                        Plugin.Instance.SyncGetMetaData(track);
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
