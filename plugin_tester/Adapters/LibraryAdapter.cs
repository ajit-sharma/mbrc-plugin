using System;
using System.Collections.Generic;
using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Feature.Library;
using MusicBeeRemote.Core.Feature.Podcasts;
using MusicBeeRemote.Core.Feature.Radio;

namespace MusicBeeRemoteTester.Adapters
{
    internal class LibraryAdapter : ILibraryApiAdapter
    {
        public IList<Track> GetTracks(string[] tracks = null)
        {
            throw new NotImplementedException();
        }

        public IList<Genre> GetGenres(string filter = "")
        {
            throw new NotImplementedException();
        }

        public IList<Album> GetAlbums(string filter = "")
        {
            throw new NotImplementedException();
        }

        public IList<Artist> GetArtists(string filter = "")
        {
            throw new NotImplementedException();
        }

        public IList<Playlist> GetPlaylists()
        {
            throw new NotImplementedException();
        }

        public IList<RadioStation> GetRadioStations()
        {
            throw new NotImplementedException();
        }

        public IList<PodcastSubscription> GetPodcastSubscriptions()
        {
            throw new NotImplementedException();
        }

        public IList<PodcastEpisode> GetEpisodes(string subscriptionId)
        {
            throw new NotImplementedException();
        }

        public byte[] GetPodcastSubscriptionArtwork(string subscriptionId)
        {
            throw new NotImplementedException();
        }

        public string GetArtistUrl(string name)
        {
            throw new NotImplementedException();
        }

        public byte[] GetCover(string path)
        {
            throw new NotImplementedException();
        }

        public string GetCoverUrl(string path)
        {
            throw new NotImplementedException();
        }

        public string[] GetLibraryFiles()
        {
            throw new NotImplementedException();
        }

        public Modifications GetSyncDelta(string[] cachedFiles, DateTime lastSync)
        {
            throw new NotImplementedException();
        }
    }
}