namespace MusicBeeRemoteCore.AndroidRemote.Interfaces
{
    internal interface ICommand
    {
        void Execute(IEvent eEvent);
    }
}