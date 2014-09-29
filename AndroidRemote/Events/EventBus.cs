#region

using MusicBeePlugin.AndroidRemote.Interfaces;

#endregion

namespace MusicBeePlugin.AndroidRemote.Events
{
    internal class EventBus
    {
        public static Controller.Controller Controller;

        public static void FireEvent(IEvent e)
        {
            Controller.CommandExecute(e);
        }
    }
}