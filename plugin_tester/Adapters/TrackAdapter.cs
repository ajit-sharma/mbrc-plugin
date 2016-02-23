namespace plugin_tester
{
    using System;

    using MusicBeePlugin.AndroidRemote.Enumerations;
    using MusicBeePlugin.AndroidRemote.Model;
    using MusicBeePlugin.ApiAdapters;
    using MusicBeePlugin.Rest.ServiceInterface;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    internal class TrackAdapter : ITrackApiAdapter
    {

        private const int DURATION = 200030303;
        private float rating;

        private int position;

        public LastfmStatus GetLoveStatus(string action)
        {
            throw new NotImplementedException();
        }

        public PositionResponse GetPosition()
        {
            return new PositionResponse
                       {
                           Code = ApiCodes.Success,
                           Position = this.position,
                           Duration = DURATION
                       };
        }

        public float GetRating()
        {
            return this.rating;
        }

        public TrackInfoResponse GetTrackInfo()
        {
            return new TrackInfoResponse
                       {
                           Album = "Panic", 
                           Artist = "Caravan Palace", 
                           Code = ApiCodes.Success, 
                           Path = "C:\\Music\\Caravan Palace\\Panic\\RockItForMe.mp3", 
                           Title = "Rock it for me", 
                           Year = "2012"
                       };
        }

        public void RequestCover(LyricCoverModel model)
        {
            throw new NotImplementedException();
        }

        public PositionResponse SetPosition(int newPosition)
        {
            this.position = newPosition <= DURATION ? newPosition : DURATION;
            return new PositionResponse
                       {
                           Code = ApiCodes.Success,
                           Duration = DURATION,
                           Position = this.position
                       };
        }

        public float SetRating(float rating)
        {
            this.rating = rating;
            return rating;
        }
    }
}