namespace MusicBeeRemoteCore.ApiAdapters
{
    using MusicBeeRemoteCore.AndroidRemote.Enumerations;
    using MusicBeeRemoteCore.AndroidRemote.Model;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

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