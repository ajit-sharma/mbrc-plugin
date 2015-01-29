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
    }
}