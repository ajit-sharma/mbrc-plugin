using System.IO;
using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Enumerations;
using MusicBeeRemote.Core.Model;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;
using NLog;

namespace MusicBeeRemote.Core.Modules
{
    public class TrackModule
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly LyricCoverModel _model;

        private readonly ITrackApiAdapter _api;

        public TrackModule(ITrackApiAdapter api, LyricCoverModel model)
        {
            _api = api;
            _model = model;
        }

        public Stream GetBinaryCoverData()
        {
            return Utilities.Utilities.GetCoverStreamFromBase64(_model.Cover);
        }

        public PositionResponse GetPosition()
        {
            return _api.GetPosition();
        }

        /// <summary>
        ///     If the given rating string is not null or empty and the value of the string is a float number in the [0,5]
        ///     the function will set the new rating as the current index's new index rating. In any other case it will
        ///     just return the rating for the current index.
        /// </summary>
        /// <returns>Track Rating</returns>
        public float GetRating()
        {
            return _api.GetRating();
        }

        public TrackInfoResponse GetTrackInfo()
        {
            return _api.GetTrackInfo();
        }

        /// <summary>
        ///     This function is used to change the playing index's last.fm love rating.
        /// </summary>
        /// <param name="action">
        ///     The action can be either love, or ban.
        /// </param>
        public LastfmStatus RequestLoveStatus(string action)
        {
            return _api.GetLoveStatus(action);
        }

        /// <summary>
        ///     Requests the Now Playing Track Cover. If the cover is available it is dispatched along with an event.
        ///     If not, and the ApiRevision is equal or greater than r17 a request for the downloaded artwork is
        ///     initiated. The cover is dispatched along with an event when ready.
        /// </summary>
        public void RequestNowPlayingTrackCover()
        {
            // todo: not sure if this will work refactor if required
            _api.RequestCover(_model);
        }

        /// <summary>
        ///     Sets the position of the playing track
        /// </summary>
        /// <param name="newPosition"></param>
        public PositionResponse SetPosition(int newPosition)
        {
            return _api.SetPosition(newPosition);
        }

        public float SetRating(float rating)
        {
            return _api.SetRating(rating);
        }
    }
}