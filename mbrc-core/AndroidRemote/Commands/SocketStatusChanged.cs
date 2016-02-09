namespace MusicBeePlugin.AndroidRemote.Commands
{
    using MusicBeePlugin.AndroidRemote.Interfaces;

    internal class SocketStatusChanged : ICommand
    {
        public void Execute(IEvent eEvent)
        {
            // Plugin.Instance.UpdateWindowStatus((Boolean)eEvent.Data);
        }
    }
}