#region

using MusicBeePlugin.AndroidRemote.Commands;
using MusicBeePlugin.AndroidRemote.Events;

#endregion

namespace MusicBeePlugin.AndroidRemote
{
    internal class Configuration
    {
        public static void Register(Controller.Controller controller)
        {
            controller.AddCommand(MessageEvent.ActionSocketStart, typeof(StartSocketServer));
            controller.AddCommand(MessageEvent.ActionSocketStop, typeof(StopSocketServer));
            controller.AddCommand(MessageEvent.ActionClientConnected, typeof(ClientConnected));
            controller.AddCommand(MessageEvent.ActionClientDisconnected, typeof(ClientDisconnected));
            controller.AddCommand(MessageEvent.ActionForceClientDisconnect, typeof(ForceClientDisconnect));
            controller.AddCommand(MessageEvent.StartServiceBroadcast, typeof(StartServiceBroadcast));
            controller.AddCommand(MessageEvent.SocketStatusChange, typeof(SocketStatusChanged));
            controller.AddCommand(MessageEvent.RestartSocket, typeof(RestartSocketCommand));
            controller.AddCommand(MessageEvent.ShowFirstRunDialog, typeof(ShowFirstRunDialogCommand));
            controller.AddCommand(MessageEvent.Notify, typeof(NotifyClient));
        }
    }
}