using MusicBeePlugin.AndroidRemote.Interfaces;
using MusicBeePlugin.AndroidRemote.Settings;

namespace MusicBeePlugin.AndroidRemote.Commands.Internal
{
    class ShowFirstRunDialogCommand : ICommand
    {
        public void Dispose()
        {
        }

        public void Execute(IEvent eEvent)
        {
            if (false)
            {
                Plugin.Instance.OpenInfoWindow();
            }
        }
    }
}
