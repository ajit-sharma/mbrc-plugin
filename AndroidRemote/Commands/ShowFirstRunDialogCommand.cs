#region

using MusicBeePlugin.AndroidRemote.Interfaces;

#endregion

namespace MusicBeePlugin.AndroidRemote.Commands
{
    internal class ShowFirstRunDialogCommand : ICommand
    {
        public void Execute(IEvent eEvent)
        {
            if (false)
            {
                Plugin.Instance.OpenInfoWindow();
            }
        }
    }
}