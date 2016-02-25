namespace MusicBeeRemoteCore.ApiAdapters
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Forms;

    using MusicBeeRemoteCore.AndroidRemote.Enumerations;
    using MusicBeeRemoteCore.AndroidRemote.Model;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;
    
    class TrackApiAdapter : ITrackApiAdapter
    {
  
        private readonly Plugin.MusicBeeApiInterface api;

        public TrackApiAdapter(Plugin.MusicBeeApiInterface api)
        {
            this.api = api;
        }

        public LastfmStatus GetLoveStatus(string action)
        {
            var hwnd = this.api.MB_GetWindowHandle();
            var mb = (Form)Control.FromHandle(hwnd);

            if (action.Equals("toggle", StringComparison.OrdinalIgnoreCase))
            {
                if (this.GetLfmStatus() == LastfmStatus.love || this.GetLfmStatus() == LastfmStatus.ban)
                {
                    mb.Invoke(new MethodInvoker(this.SetLfmNormalStatus));
                }
                else
                {
                    mb.Invoke(new MethodInvoker(this.SetLfmLoveStatus));
                }
            }
            else if (action.Equals("love", StringComparison.OrdinalIgnoreCase))
            {
                mb.Invoke(new MethodInvoker(this.SetLfmLoveStatus));
            }
            else if (action.Equals("ban", StringComparison.OrdinalIgnoreCase))
            {
                mb.Invoke(new MethodInvoker(this.SetLfmLoveBan));
            }
            else if (action.Equals("normal", StringComparison.OrdinalIgnoreCase))
            {
                mb.Invoke(new MethodInvoker(this.SetLfmNormalStatus));
            }

            return this.GetLfmStatus();
        }

        public PositionResponse GetPosition()
        {
            var currentPosition = this.api.Player_GetPosition();
            var totalDuration = this.api.NowPlaying_GetDuration();

            return new PositionResponse { Position = currentPosition, Duration = totalDuration };
        }

        public float GetRating()
        {
            float rating = -1;
            try
            {
                var sRate = this.api.Library_GetFileTag(this.api.NowPlaying_GetFileUrl(), Plugin.MetaDataType.Rating);
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
                                Artist = this.api.NowPlaying_GetFileTag(Plugin.MetaDataType.Artist), 
                                Album = this.api.NowPlaying_GetFileTag(Plugin.MetaDataType.Album), 
                                Year = this.api.NowPlaying_GetFileTag(Plugin.MetaDataType.Year), 
                                Title = this.api.NowPlaying_GetFileTag(Plugin.MetaDataType.TrackTitle), 
                                Path = this.api.NowPlaying_GetFileUrl()
                            };
            return track;
        }

        public void RequestCover(LyricCoverModel model)
        {
            if (!string.IsNullOrEmpty(this.api.NowPlaying_GetArtwork()))
            {
                this.api.NowPlaying_GetArtwork();
            }
            else if (this.api.ApiRevision >= 17)
            {
                model.SetCover(this.api.NowPlaying_GetDownloadedArtwork());
            }
        }

        public PositionResponse SetPosition(int newPosition)
        {
            this.api.Player_SetPosition(newPosition);

            var currentPosition = this.api.Player_GetPosition();
            var totalDuration = this.api.NowPlaying_GetDuration();

            return new PositionResponse { Position = currentPosition, Duration = totalDuration };
        }

        public float SetRating(float rating)
        {
            try
            {
                if (rating >= 0 && rating <= 5)
                {
                    var newRating = rating.ToString(CultureInfo.InvariantCulture);
                    this.api.Library_SetFileTag(this.api.NowPlaying_GetFileUrl(), Plugin.MetaDataType.Rating, newRating);
                    this.api.Library_CommitTagsToFile(this.api.NowPlaying_GetFileUrl());
                    this.api.Player_GetShowRatingTrack();
                    this.api.MB_RefreshPanels();
                }

                var sRate = this.api.Library_GetFileTag(this.api.NowPlaying_GetFileUrl(), Plugin.MetaDataType.Rating);
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
            var apiReply = this.api.NowPlaying_GetFileTag(Plugin.MetaDataType.RatingLove);
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
            var fileUrl = this.api.NowPlaying_GetFileUrl();
            this.api.Library_SetFileTag(fileUrl, Plugin.MetaDataType.RatingLove, "Blfm");
        }

        private void SetLfmLoveStatus()
        {
            this.api.Library_SetFileTag(this.api.NowPlaying_GetFileUrl(), Plugin.MetaDataType.RatingLove, "Llfm");
        }

        private void SetLfmNormalStatus()
        {
            this.api.Library_SetFileTag(this.api.NowPlaying_GetFileUrl(), Plugin.MetaDataType.RatingLove, "lfm");
        }
    }
}