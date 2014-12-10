#region

using MusicBeePlugin.AndroidRemote.Interfaces;
using MusicBeePlugin.AndroidRemote.Utilities;

#endregion

namespace MusicBeePlugin.AndroidRemote.Commands
{
    internal class ClientDisconnected : ICommand
    {
        public void Execute(IEvent eEvent)
        {
            Authenticator.RemoveClientOnDisconnect(eEvent.ClientId);
        }
    }
}