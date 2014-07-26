using MusicBeePlugin.AndroidRemote.Interfaces;

namespace MusicBeePlugin.AndroidRemote.Commands.Internal
{
    class InitializeModelStateCommand:ICommand
    {
        public void Dispose()
        {
            
        }

        public void Execute(IEvent eEvent)
        {
            var plugin = Plugin.Instance;
            
        }
    }
}
