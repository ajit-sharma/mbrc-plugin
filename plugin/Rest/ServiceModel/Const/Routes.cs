namespace MusicBeePlugin.Rest.ServiceModel.Const
{
    /// <summary>
    ///     The Routes of the API
    /// </summary>
    public class Routes
    {
        #region Player Route API
        public const string PlayerShuffle = "/player/shuffle";
        public const string PlayerScrobble = "/player/scrobble";
        public const string PlayerRepeat = "/player/repeat";
        public const string PlayerMute = "/player/mute";
        public const string PlayerVolume = "/player/volume";
        public const string PlayerAutodj = "/player/autodj";
        public const string PlayerPlaystate = @"/player/playstate";
        public const string PlayerPrevious = "/player/previous";
        public const string PlayerNext = "/player/next";
        public const string PlayerPlay = "/player/play";
        public const string PlayerStop = "/player/stop";
        public const string PlayerPause = "/player/pause";
        public const string PlayerPlaypause = "/player/playpause";
        public const string PlayerStatus = "/player/status";
        #endregion

        #region Playing Track API
        public const string Track = "/track";
        public const string TrackRating = "/track/rating";
        public const string TrackPosition = "/track/position";
        public const string TrackCover = "/track/cover/";
        public const string TrackLyrics = "/track/lyrics";
        public const string TrackLyricsRaw = "/track/lyrics/raw";
        #endregion

	    public const string Playlists = "/playlists";
	    public const string PlaylistsPlay = "/playlists/play";
	    public const string PlaylistsId = "/playlists/{id}";
	    public const string PlaylistsIdTracks = "/playlists/{id}/tracks";
	    public const string PlaylistsIdTracksMove = "/playlists/{id}/tracks/move";
    }
}