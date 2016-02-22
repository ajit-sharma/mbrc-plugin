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
        public LastfmStatus GetLoveStatus(string action)
        {
            throw new NotImplementedException();
        }

        public PositionResponse GetPosition()
        {
            throw new NotImplementedException();
        }

        public float GetRating()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public float SetRating(float rating)
        {
            throw new NotImplementedException();
        }
    }
}