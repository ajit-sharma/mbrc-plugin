namespace MusicBeePlugin.Rest.ServiceModel.Const
{
    /// <summary>
    ///     Short Summary of the API functionality
    /// </summary>
    public class Summary
    {
	    public const string RatingPut = @"Sets the track rating for the playing track";
        public const string ShuffleGet = @"Gets the current state of shuffle.";
        public const string ShufflePut = @"Sets the shuffle status.";
        public const string ScrobbleGet = @"Gets the status of last.fm scrobbling";
        public const string ScrobbleSet = @"Sets the status of last.fm scrobbling.";
        public const string RatingGet = @"Gets the track rating for the playing track.";
        public const string TrackGet = "Gets the information of the playing track";
        public const string Cover = "Retrieves the playing track's cover image (jpeg).";
        public const string LyricsRaw = "Returns the lyrics of the playing track as plain text.";
        public const string TrackPositionGet = @"Gets the current position and total duration of the playing track.";
        public const string TrackPositionSet = @"Sets a new position for the playing track";
        public const string LyricsGet = "Gets a lyrics response object containing a lyrics property";
	    public const string PlaylistGet = @"Gets the available playlists";
	    public const string PlaylistPut = @"Creates a new playlist.";
	    public const string PlaylistPlay = @"Adds a playlist for direct playback.";
	    public const string PlaylistTrackDelete = @"Removes a track from a playlist.";
	    public const string PlaylistTrackMove = @"Changes the position of a track in the playlist.";
	    public const string PlaylistTrackAdd = @"Adds tracks to an existing playlist";
	    public const string DeletesAPlaylist = @"Deletes a playlist";
	    public const string PlaylistTracks = @"Gets the tracks of a specified playlist.";
	    public const string GetsAPlaylist = @"Gets a playlist";
	    public const string GetRepeat = @"Gets the active repeat mode.";
	    public const string RepeatPut = @"Sets the player repeat mode, or toggles it.";
	    public const string MuteGet = @"Gets the mute state of the player.";
	    public const string MutePut = @"Sets the mute state of the player.";
	    public const string VolumeGet = @"Gets the volume.";
	    public const string VolumePut = @"Sets the current volume.";
	    public const string AutoDjGet = @"Gets the autodj state of the player.";
	    public const string PlayerAction = @"Depending on the action passed it can play, pause, play next, previous and stop playback.";
	    public const string PlayerStatusGet = @"Gets the status of the player.";
	    public const string PlaystateGet = @"Gets the current play state.";
	    public const string AutoDjPut = @"Sets the autodj state";
	    public const string NowPlayingGet = "Retrieves the tracks in the now playing list.";
	    public const string NowPlayingPlay = "Plays the track specified.";
	    public const string NowPlayingDelete = "Removes a track from the now playing list.";
	    public const string NowPlayingMove = "Moves a track from a position in the now playing list to another.";
	    public const string NowPlayingQueue = "Queues item for playing in the now playing queue.";
	    public const string LibraryTracksGet = "Retrieves the library tracks stored in the database";
	    public const string LibraryTrackByIdGet = "Retrieves a track matching the specified id from the library";
	    public const string LibraryArtist = "Retrieves the artists stored in the database";
	    public const string LibraryArtistById = "Retrieves a single artist entry that matches the specified id.";
	    public const string LibraryAlbums = "Retrieves the albums stored in the database";
	    public const string LibraryAlbumsId = "Retrieves a single album from the database.";
	    public const string LibraryGenres = "Retrieves the genres stored in the database.";
	    public const string LibraryGenresId = "Retrieves as single genre from the database.";
	    public const string LibraryCovers = "Retrieves the cover entries stored in the database.";
	    public const string LibraryCoversId = "Retrieves a single cover from the database.";
	    public const string LibraryCoversIdRaw = "Retrieves a jpeg cover image from the cache.";
	    public const string PlaylistChanges = @"Gets the changes in the playlist.";
	    public const string PlaylistTrackInfo = "Retrieves playlist track info.";
	    public const string LibrayTracksU = @"Gets the changes on the track table.";
		public const string LibraryArtistU = @"Gets the changes on the artist table.";
		public const string LibraryAlbumsU = @"Gets the changes on the albums table.";
		public const string LibraryGenresU = @"Gets the changes on the genres table.";
		public const string LibraryCoversU = @"Gets the changes on the covers table.";
	    public const string PlaylistTrackChanges = @"Gets the changes in the playlist tracks.";
	    public const string PlaylistTrackInfoChanges = @"Gets the changes in the playlist track info.";
    }
}