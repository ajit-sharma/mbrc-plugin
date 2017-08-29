using System;
using System.Collections.Generic;
using MusicBeeRemote.Core.Feature.Library;
using MusicBeeRemote.Core.Feature.Podcasts;
using MusicBeeRemote.Core.Feature.Radio;

namespace MusicBeeRemote.Core.ApiAdapters
{
    public interface ILibraryApiAdapter
    {
        IList<Track> GetTracks();

        IList<Genre> GetGenres(string filter = "");

        IList<Album> GetAlbums(string filter = "");

        IList<Artist> GetArtists(string filter = "");
        
        IList<Playlist> GetPlaylists();

        IList<RadioStation> GetRadioStations();

        IList<PodcastSubscription> GetPodcastSubscriptions();

        IList<PodcastEpisode> GetEpisodes(string subscriptionId);

        byte[] GetPodcastSubscriptionArtwork(string subscriptionId);
                     
        string GetArtistUrl(string name);

        byte[] GetCover(string path);

        string GetCoverUrl(string path);       

        string[] GetLibraryFiles();

        Modifications GetSyncDelta(string[] cachedFiles, DateTime lastSync);    
    }
}