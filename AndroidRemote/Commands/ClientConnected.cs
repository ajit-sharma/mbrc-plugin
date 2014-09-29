#region

using MusicBeePlugin.AndroidRemote.Interfaces;
using MusicBeePlugin.AndroidRemote.Utilities;

#endregion

namespace MusicBeePlugin.AndroidRemote.Commands
{
    internal class ClientConnected : ICommand
    {
        public void Execute(IEvent eEvent)
        {
            Authenticator.AddClientOnConnect(eEvent.ClientId);
        }
    }
}