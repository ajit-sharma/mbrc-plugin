using System;
using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Enumerations;
using MusicBeeRemote.Core.Model;
using MusicBeeRemote.Core.Rest.ServiceInterface;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;

namespace MusicBeeRemoteTester.Adapters
{
    internal class TrackAdapter : ITrackApiAdapter
    {
        private const int DURATION = 200030303;

        private int position;

        private float rating;

        public LastfmStatus GetLoveStatus(string action)
        {
            throw new NotImplementedException();
        }

        public PositionResponse GetPosition()
        {
            return new PositionResponse { Code = ApiCodes.Success, Position = position, Duration = DURATION };
        }

        public float GetRating()
        {
            return rating;
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
            position = newPosition <= DURATION ? newPosition : DURATION;
            return new PositionResponse { Code = ApiCodes.Success, Duration = DURATION, Position = position };
        }

        public float SetRating(float rating)
        {
            this.rating = rating;
            return rating;
        }
    }
}