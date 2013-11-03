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
            bool isTrue = false;

            bool.TryParse(eEvent.Data.ToString(), out isTrue);
            
            if (isTrue)
            {
                Plugin.Instance.CheckForLibaryChanges();
            }
            else
            {
                Plugin.Instance.SyncLibrary();    
            }
             
            
        }
    }
}
