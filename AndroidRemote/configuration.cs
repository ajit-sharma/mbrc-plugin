namespace MusicBeePlugin.AndroidRemote
{
    using Commands.Internal;
    using Events;

    internal class Configuration
    {
        public static void Register(Controller.Controller controller)
        {
            controller.AddCommand(EventType.ActionSocketStart, typeof (StartSocketServer));
            controller.AddCommand(EventType.ActionSocketStop, typeof (StopSocketServer));
            controller.AddCommand(EventType.ActionClientConnected, typeof (ClientConnected));
            controller.AddCommand(EventType.ActionClientDisconnected, typeof (ClientDisconnected));
            controller.AddCommand(EventType.ActionForceClientDisconnect, typeof (ForceClientDisconnect));
            controller.AddCommand(EventType.StartServiceBroadcast, typeof(StartServiceBroadcast));
            controller.AddCommand(EventType.SocketStatusChange, typeof(SocketStatusChanged));
            controller.AddCommand(EventType.RestartSocket, typeof(RestartSocketCommand));
            controller.AddCommand(EventType.ShowFirstRunDialog, typeof(ShowFirstRunDialogCommand));
        }
    }
}