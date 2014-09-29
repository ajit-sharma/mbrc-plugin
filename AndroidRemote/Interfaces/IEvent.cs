namespace MusicBeePlugin.AndroidRemote.Interfaces
{
    public interface IEvent
    {
        object Data { get; }
        string Type { get; }
        string ClientId { get; }
        string GetDataString();
    }
}
