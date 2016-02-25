namespace MusicBeeRemoteCore.AndroidRemote.Commands
{
    using MusicBeeRemoteCore.AndroidRemote.Interfaces;

    internal class SocketStatusChanged : ICommand
    {
        public void Execute(IEvent eEvent)
        {
            // Plugin.Instance.UpdateWindowStatus((Boolean)eEvent.Data);
        }
    }
}