namespace MusicBeePlugin
{
    using System;
    using System.Collections.Generic;

    using MusicBeePlugin.AndroidRemote.Enumerations;
    using MusicBeePlugin.Model;
    using MusicBeePlugin.Rest.ServiceModel.Enum;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface IPlayerApiAdapter
    {
        ICollection<LibraryAlbum> GetAlbumList();

        ICollection<LibraryArtist> GetArtistList();

        string GetArtistUrl(string name);

        byte[] GetCoverData(string path);

        string GetCoverUrl(string path);

        ICollection<LibraryGenre> GetGenreList();

        string[] GetLibraryFiles();

        Modifications GetSyncDelta(string[] cachedFiles, DateTime lastSync);

        LibraryTrackEx GetTags(string file);

        bool NowPlayingMoveTrack(int @from, int to);

        bool NowPlayingRemove(int index);

        bool QueueNow(string[] tracklist);

        bool QueueLast(string[] tracklist);

        bool QueueNext(string[] tracklist);

        bool PlayNow(string path);

        ICollection<NowPlaying> GetNowPlayingList();

        /// <summary>
        /// Plays the next track available in the queue and returns true on success or
        /// false on failure.
        /// </summary>
        /// <returns>
        /// The success of the request <see cref="bool"/>. 
        /// </returns>
        bool PlayNext();

        /// <summary>
        /// Gets the available output devices from the API
        /// </summary>
        /// <returns>
        /// The <see cref="OutputDevice"/>.
        /// </returns>
        OutputDevice GetOutputDevices();

        bool SetOutputDevice(string active);

        bool StopPlayback();

        bool ToggleShuffle();

        bool ChangeRepeat();

        bool PlayPause();

        PlayerStatus GetStatus();

        bool PlayPrevious();

        int GetVolume();

        bool SetVolume(int volume);

        string GetPlayState();

        bool SetShuffleState(ShuffleState state);

        ShuffleState GetShuffleState();

        bool SetRepeatState(ApiRepeatMode mode);

        bool StartPlayback();

        bool PausePlayback();

        bool ChangeAutoDj(bool enabled);

        bool GetAutoDjState();

        string GetRepeatState();

        bool SetScrobbleState(bool enabled);

        bool GetScrobbleState();

        bool GetMuteState();

        bool SetMute(bool enabled);
    }
}