using MusicBeeRemote.Core.Enumerations;
using MusicBeeRemote.Core.Model;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;

namespace MusicBeeRemote.Core.ApiAdapters
{
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