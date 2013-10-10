using MusicBeePlugin.AndroidRemote.Interfaces;

namespace MusicBeePlugin.AndroidRemote.Commands.Requests
{
    class RequestPlaylistFiles:ICommand
    {
        public void Dispose()
        {
        }

        public void Execute(IEvent eEvent)
        {
            Plugin.Instance.GetTracksForPlaylist(eEvent.DataToString(), eEvent.ClientId);
        }
    }
}
