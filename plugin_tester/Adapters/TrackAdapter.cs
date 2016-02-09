namespace plugin_tester
{
    using System;

    using MusicBeePlugin.AndroidRemote.Enumerations;
    using MusicBeePlugin.AndroidRemote.Model;
    using MusicBeePlugin.ApiAdapters;
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
            throw new NotImplementedException();
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