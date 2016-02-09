namespace MusicBeePlugin.AndroidRemote.Interfaces
{
    public interface IEvent
    {
        string ClientId { get; }

        object Data { get; }

        string Type { get; }

        string GetDataString();
    }
}