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
        public const string PlayerAction = "/player/action";
        public const string PlayerStatus = "/player/status";
        public const string PlayerOutput = "/player/output";
        #endregion

        #region Playing Track API
        public const string Track = "/track";
        public const string TrackRating = "/track/rating";
        public const string TrackPosition = "/track/position";
        public const string TrackCover = "/track/cover/";
        public const string TrackLyrics = "/track/lyrics";
        public const string TrackLyricsRaw = "/track/lyrics/raw";
		#endregion

		#region Playlist API
		public const string Playlists = "/playlists";
	    public const string PlaylistsPlay = "/playlists/play";
	    public const string PlaylistsId = "/playlists/{id}";
	    public const string PlaylistsIdTracks = "/playlists/{id}/tracks";
        public const string PlaylistsIdTracksPosition = "/playlists/{id}/tracks/{position}";
        public const string PlaylistsIdTracksMove = "/playlists/{id}/tracks/{from}/{to}";
		public const string PlaylistsChanges = "/playlists/u";
		public const string PlaylistsTrackInfo = "/playlists/trackinfo";
	    public const string PlaylistTrackInfoChanges = "/playlists/trackinfo/u";
		public const string PlaylistTrackChanges = "/playlists/track/u";

		#endregion

		#region Now Playing API
		public const string Nowplaying = "/nowplaying/";
	    public const string NowplayingPlay = "/nowplaying/play";
	    public const string NowplayingId = "/nowplaying/{id}";
	    public const string NowplayingMove = "/nowplaying/move";
	    public const string NowplayingQueue = "/nowplaying/queue";
		#endregion

		#region Library API
		public const string LibraryTracks = "/library/tracks";
	    public const string LibraryTracksId = "/library/tracks/{id}";
	    public const string LibraryArtists = "/library/artists";
	    public const string LibraryArtistsId = "/library/artists/{id}";
	    public const string LibraryAlbums = "/library/albums";
	    public const string LibraryAlbumsId = "/library/albums/{id}";
	    public const string LibraryGenres = "/library/genres";
	    public const string LibraryGenresId = "/library/genres/{id}";
	    public const string LibraryCovers = "/library/covers";
	    public const string LibraryCoversId = "/library/covers/{id}";
	    public const string LibraryCoversIdRaw = "/library/covers/{id}/raw";
		public const string LibraryTracksU = "/library/tracks/u";
		public const string LibraryArtistsU = "/library/artists/u";
		public const string LibraryAlbumsU = "/library/albums/u";
	    public const string LibraryGenresU = "/library/genres/u";
	    public const string LibraryCoversU = "/library/covers/u";
	    #endregion
    }
}