namespace MusicBeePlugin.ApiAdapters
{
    using MusicBeePlugin.AndroidRemote.Enumerations;
    using MusicBeePlugin.AndroidRemote.Model;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface ITrackApiAdapter
    {
        LastfmStatus GetLoveStatus(string action);

        PositionResponse GetPosition();

        float GetRating();

        TrackInfoResponse GetTrackInfo();

        void RequestCover(LyricCoverModel model);

        PositionResponse SetPosition(int newPosition);

        float SetRating(float rating);
    }
}