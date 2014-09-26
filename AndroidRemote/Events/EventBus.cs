
namespace MusicBeePlugin.AndroidRemote.Events
{
    using Interfaces;

    class EventBus
    {
        public static Controller.Controller Controller;

        public static void FireEvent(IEvent e)
        {
            Controller.CommandExecute(e);
        }
    }
}
