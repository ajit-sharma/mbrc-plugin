namespace MusicBeeRemoteCore.AndroidRemote.Events
{
    using MusicBeeRemoteCore.AndroidRemote.Controller;
    using MusicBeeRemoteCore.AndroidRemote.Interfaces;

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