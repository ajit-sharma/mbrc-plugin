using System;
using System.Diagnostics;
using ServiceStack.Text;

namespace MusicBeePlugin.AndroidRemote.Commands.Requests
{
    using Interfaces;
    class RequestLibrary : ICommand
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
                int offset;
                int limit;
                switch (syncType)
                {
                    case "partial":

                        string[] cachedFiles = obj.Get<string[]>("files");
                        string lastSync = obj.Get("lastsync");
                        //todo: fix partial sync call
                        break;
                    case "cover":
                        offset = obj.Get<int>("offset");
                        limit = obj.Get<int>("limit");
                        if (limit == 0)
                        {
                            limit = 50;
                        }
                        
                        break;
                    case "meta":
                        offset = obj.Get<int>("offset");
                        limit = obj.Get<int>("limit");
                        if (limit == 0)
                        {
                            limit = 50;;
                        }
                        
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
