namespace MusicBeeRemoteCore
{
    using MusicBeeRemoteCore.AndroidRemote.Enumerations;
    using MusicBeeRemoteCore.Rest.ServiceModel.Enum;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    public interface IPlayerApiAdapter
    {
        bool ChangeAutoDj(bool enabled);

        bool ChangeRepeat();

        bool GetAutoDjState();

        bool GetMuteState();

        /// <summary>
        /// Gets the available output devices from the API
        /// </summary>
        /// <returns>
        /// The <see cref="OutputDevice"/>.
        /// </returns>
        OutputDevice GetOutputDevices();

        string GetPlayState();

        string GetRepeatState();

        bool GetScrobbleState();

        Shuffle GetShuffleState();

        PlayerStatus GetStatus();

        int GetVolume();

        bool PausePlayback();

        /// <summary>
        /// Plays the next track available in the queue and returns true on success or
        /// false on failure.
        /// </summary>
        /// <returns>
        /// The success of the request <see cref="bool"/>. 
        /// </returns>
        bool PlayNext();

        bool PlayPause();

        bool PlayPrevious();

        bool SetMute(bool enabled);

        bool SetOutputDevice(string active);

        bool SetRepeatState(ApiRepeatMode mode);

        bool SetScrobbleState(bool enabled);

        bool SetShuffleState(Shuffle state);

        bool SetVolume(int volume);

        bool StartPlayback();

        bool StopPlayback();

        bool ToggleShuffle();
    }
}