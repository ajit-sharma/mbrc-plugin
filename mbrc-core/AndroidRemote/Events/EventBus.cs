namespace MusicBeePlugin.AndroidRemote.Events
{
    using MusicBeePlugin.AndroidRemote.Controller;
    using MusicBeePlugin.AndroidRemote.Interfaces;

    public class EventBus
    {
        private Controller controller;

        public EventBus(Controller controller)
        {
            this.controller = controller;
        }

        public void Publish(IEvent e)
        {
            this.controller.CommandExecute(e);
        }
    }
}