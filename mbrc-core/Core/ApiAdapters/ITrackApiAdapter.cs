using MusicBeeRemote.Core.Feature;
using MusicBeeRemote.Core.Feature.Player;
using MusicBeeRemote.Core.Network.Http.Responses.Type;

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