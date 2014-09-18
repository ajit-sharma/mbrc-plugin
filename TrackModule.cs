using System;
using System.Globalization;
using System.Windows.Forms;
using MusicBeePlugin.AndroidRemote.Enumerations;
using MusicBeePlugin.Rest.ServiceModel;
using NLog;

namespace MusicBeePlugin
{
    public class TrackModule
    {
        private Plugin.MusicBeeApiInterface _api;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public TrackModule(Plugin.MusicBeeApiInterface api)
        {
            _api = api;
        }
        public Rest.ServiceModel.Type.Track GetTrackInfo()
        {
            var track = new Rest.ServiceModel.Type.Track
            {
                Artist = _api.NowPlaying_GetFileTag(Plugin.MetaDataType.Artist),
                Album = _api.NowPlaying_GetFileTag(Plugin.MetaDataType.Album),
                Year = _api.NowPlaying_GetFileTag(Plugin.MetaDataType.Year),
                Title = _api.NowPlaying_GetFileTag(Plugin.MetaDataType.TrackTitle)
            };
            //, _api.NowPlaying_GetFileUrl()
            return track;
        }
        /// <summary>
        /// If the given rating string is not null or empty and the value of the string is a float number in the [0,5]
        /// the function will set the new rating as the current index's new index rating. In any other case it will
        /// just return the rating for the current index.
        /// </summary>
        /// <returns>Track Rating</returns>
        public float GetRating()
        {
            float rating = -1;
            try
            {
                var sRate = _api.Library_GetFileTag(_api.NowPlaying_GetFileUrl(), Plugin.MetaDataType.Rating);
                float.TryParse(sRate, out rating);
            }
            catch (Exception e)
            {
                Logger.DebugException("Exception", e);
            }
            return rating;
        }

        public float SetRating(float rating)
        {
            try
            {
                if (rating >= 0 && rating <= 5)
                {
                    _api.Library_SetFileTag(_api.NowPlaying_GetFileUrl(), Plugin.MetaDataType.Rating,
                        rating.ToString(CultureInfo.InvariantCulture));
                    _api.Library_CommitTagsToFile(_api.NowPlaying_GetFileUrl());
                    _api.Player_GetShowRatingTrack();
                    _api.MB_RefreshPanels();
                }
                var sRate = _api.Library_GetFileTag(_api.NowPlaying_GetFileUrl(), Plugin.MetaDataType.Rating);
                float.TryParse(sRate, out rating);
            }
            catch (Exception e)
            {
                Logger.DebugException("Exception", e);
            }
            return rating;
        }

        /// <summary>
        /// Requests the Now Playing Track Cover. If the cover is available it is dispatched along with an event.
        /// If not, and the ApiRevision is equal or greater than r17 a request for the downloaded artwork is
        /// initiated. The cover is dispatched along with an event when ready.
        /// </summary>
        public void RequestNowPlayingTrackCover()
        {
            if (!String.IsNullOrEmpty(_api.NowPlaying_GetArtwork()))
            {
                _api.NowPlaying_GetArtwork();
            }
            else if (_api.ApiRevision >= 17)
            {
                var cover = _api.NowPlaying_GetDownloadedArtwork();
            }
            else
            {
                
            }
        }

        /// <summary>
        /// Sets the position of the playing track
        /// </summary>
        /// <param name="newPosition"></param>
        public TrackPositionResponse SetPosition(int newPosition)
        {
            _api.Player_SetPosition(newPosition);

            var currentPosition = _api.Player_GetPosition();
            var totalDuration = _api.NowPlaying_GetDuration();

            return new TrackPositionResponse()
            {
                position = currentPosition,
                duration = totalDuration
            };
        }

        public TrackPositionResponse GetPosition()
        {
            var currentPosition = _api.Player_GetPosition();
            var totalDuration = _api.NowPlaying_GetDuration();

            return new TrackPositionResponse()
            {
                position = currentPosition,
                duration = totalDuration
            };
        }

        /// <summary>
        /// This function is used to change the playing index's last.fm love rating.
        /// </summary>
        /// <param name="action">
        /// The action can be either love, or ban.
        /// </param>
        public void RequestLoveStatus(string action)
        {
            var hwnd = _api.MB_GetWindowHandle();
            var mb = (Form)Control.FromHandle(hwnd);

            if (action.Equals("toggle", StringComparison.OrdinalIgnoreCase))
            {
                if (GetLfmStatus() == LastfmStatus.Love || GetLfmStatus() == LastfmStatus.Ban)
                {
                    mb.Invoke(new MethodInvoker(SetLfmNormalStatus));
                }
                else
                {
                    mb.Invoke(new MethodInvoker(SetLfmLoveStatus));
                }
            }
            else if (action.Equals("love", StringComparison.OrdinalIgnoreCase))
            {
                mb.Invoke(new MethodInvoker(SetLfmLoveStatus));
            }
            else if (action.Equals("ban", StringComparison.OrdinalIgnoreCase))
            {
                mb.Invoke(new MethodInvoker(SetLfmLoveBan));
            }

            //SendSocketMessage(Constants.NowPlayingLfmRating, Constants.Reply, GetLfmStatus());
        }

        private void SetLfmNormalStatus()
        {
            _api.Library_SetFileTag(_api.NowPlaying_GetFileUrl(), Plugin.MetaDataType.RatingLove, "lfm");
        }

        private void SetLfmLoveStatus()
        {
            _api.Library_SetFileTag(_api.NowPlaying_GetFileUrl(), Plugin.MetaDataType.RatingLove, "Llfm");
        }

        private void SetLfmLoveBan()
        {
            var fileUrl = _api.NowPlaying_GetFileUrl();
            _api.Library_SetFileTag(fileUrl, Plugin.MetaDataType.RatingLove, "Blfm");
        }

        private LastfmStatus GetLfmStatus()
        {
            LastfmStatus lastfmStatus;
            var apiReply = _api.NowPlaying_GetFileTag(Plugin.MetaDataType.RatingLove);
            if (apiReply.Equals("L") || apiReply.Equals("lfm") || apiReply.Equals("Llfm"))
            {
                lastfmStatus = LastfmStatus.Love;
            }
            else if (apiReply.Equals("B") || apiReply.Equals("Blfm"))
            {
                lastfmStatus = LastfmStatus.Ban;
            }
            else
            {
                lastfmStatus = LastfmStatus.Normal;
            }
            return lastfmStatus;
        }

    }


}
