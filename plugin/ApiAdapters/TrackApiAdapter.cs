
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Enumerations;
using MusicBeeRemote.Core.Model;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;

namespace MusicBeePlugin.ApiAdapters
{
    class TrackApiAdapter : ITrackApiAdapter
    {
        private readonly Plugin.MusicBeeApiInterface _api;

        public TrackApiAdapter(Plugin.MusicBeeApiInterface api)
        {
            _api = api;
        }

        public LastfmStatus GetLoveStatus(string action)
        {
            var hwnd = _api.MB_GetWindowHandle();
            var mb = (Form)Control.FromHandle(hwnd);

            if (action.Equals("toggle", StringComparison.OrdinalIgnoreCase))
            {
                if (GetLfmStatus() == LastfmStatus.love || GetLfmStatus() == LastfmStatus.ban)
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
            else if (action.Equals("normal", StringComparison.OrdinalIgnoreCase))
            {
                mb.Invoke(new MethodInvoker(SetLfmNormalStatus));
            }

            return GetLfmStatus();
        }

        public PositionResponse GetPosition()
        {
            var currentPosition = _api.Player_GetPosition();
            var totalDuration = _api.NowPlaying_GetDuration();

            return new PositionResponse { Position = currentPosition, Duration = totalDuration };
        }

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
                Debug.WriteLine(e);
            }

            return rating;
        }

        public TrackInfoResponse GetTrackInfo()
        {
            var track = new TrackInfoResponse
                            {
                                Artist = _api.NowPlaying_GetFileTag(Plugin.MetaDataType.Artist), 
                                Album = _api.NowPlaying_GetFileTag(Plugin.MetaDataType.Album), 
                                Year = _api.NowPlaying_GetFileTag(Plugin.MetaDataType.Year), 
                                Title = _api.NowPlaying_GetFileTag(Plugin.MetaDataType.TrackTitle), 
                                Path = _api.NowPlaying_GetFileUrl()
                            };
            return track;
        }

        public void RequestCover(LyricCoverModel model)
        {
            if (!string.IsNullOrEmpty(_api.NowPlaying_GetArtwork()))
            {
                _api.NowPlaying_GetArtwork();
            }
            else if (_api.ApiRevision >= 17)
            {
                model.SetCover(_api.NowPlaying_GetDownloadedArtwork());
            }
        }

        public PositionResponse SetPosition(int newPosition)
        {
            _api.Player_SetPosition(newPosition);

            var currentPosition = _api.Player_GetPosition();
            var totalDuration = _api.NowPlaying_GetDuration();

            return new PositionResponse { Position = currentPosition, Duration = totalDuration };
        }

        public float SetRating(float rating)
        {
            try
            {
                if (rating >= 0 && rating <= 5)
                {
                    var newRating = rating.ToString(CultureInfo.InvariantCulture);
                    _api.Library_SetFileTag(_api.NowPlaying_GetFileUrl(), Plugin.MetaDataType.Rating, newRating);
                    _api.Library_CommitTagsToFile(_api.NowPlaying_GetFileUrl());
                    _api.Player_GetShowRatingTrack();
                    _api.MB_RefreshPanels();
                }

                var sRate = _api.Library_GetFileTag(_api.NowPlaying_GetFileUrl(), Plugin.MetaDataType.Rating);
                float.TryParse(sRate, out rating);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception");
            }

            return rating;
        }

        private LastfmStatus GetLfmStatus()
        {
            LastfmStatus lastfmStatus;
            var apiReply = _api.NowPlaying_GetFileTag(Plugin.MetaDataType.RatingLove);
            if (apiReply.Equals("L") || apiReply.Equals("lfm") || apiReply.Equals("Llfm"))
            {
                lastfmStatus = LastfmStatus.love;
            }
            else if (apiReply.Equals("B") || apiReply.Equals("Blfm"))
            {
                lastfmStatus = LastfmStatus.ban;
            }
            else
            {
                lastfmStatus = LastfmStatus.normal;
            }

            return lastfmStatus;
        }

        private void SetLfmLoveBan()
        {
            var fileUrl = _api.NowPlaying_GetFileUrl();
            _api.Library_SetFileTag(fileUrl, Plugin.MetaDataType.RatingLove, "Blfm");
        }

        private void SetLfmLoveStatus()
        {
            _api.Library_SetFileTag(_api.NowPlaying_GetFileUrl(), Plugin.MetaDataType.RatingLove, "Llfm");
        }

        private void SetLfmNormalStatus()
        {
            _api.Library_SetFileTag(_api.NowPlaying_GetFileUrl(), Plugin.MetaDataType.RatingLove, "lfm");
        }
    }
}