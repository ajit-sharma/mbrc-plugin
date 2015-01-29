namespace MusicBeePlugin.Rest.ServiceModel.Const
{
    /// <summary>
    ///     Holds descriptions of various parameters
    /// </summary>
    public class Description
    {
        public const string Mute = @"If the value is true or false it will mute/unmute the audio." +
                                   "\n If left empty it will toggle mute on/off depending on the previous state.";

        public const string ShuffleEnabled = @"If the value is true or false it will enable/disable shuffle." +
                                             "\n If left empty it will toggle shuffle on/off depending on the previous state.";

        public const string ScrobbleEnabled =
            @"If the value is true or false it will enable/disable last.fm scrobbling." +
            "\n If left empty it will toggle scrobbling on/off depending on the previous state.";

        public const string RepeatMode =
            @"It can be all/none or empty. If left empty it will change between the available states."
            + @" [Note: As far as I know repeat one is not supported by the MusicBee API.";

        public const string Volume = @"The new volume of the player ranges from 0 (no sound) to 100 (maximum)";
        public const string Rating = @"The track rating is optional and can have values starting from 0 up to 5 with half steps (.5) included."
                                    + "If left null it will remove the rating from the track.";

        public const string Position = "The position of the track as an integer represented in milliseconds";
	    public const string Limit = "The number of results contained in the response.";
	    public const string Offset = "The position of the first entry in the response.";
	    public const string PlaylistName = @"The name of the new playlist to be created";
	    public const string PlaylistList = @"The list of the tracks to add to the new playlist.";
	    public const string PlaylistPlay = @"The path in the filesystem of the playlist to play.";
	    public const string IdDesc = @"The id of the entry to retrieve";
	    public const string PlaylistIdDesc = @"The id of the playlist which contains the tracks.";
	    public const string PlaylistTrackPosition = @"The position of the track in the playlist.";
	    public const string MoveFrom = @"The old position of the track in the playlist.";
	    public const string MoveTo = @"The new position of the track in the playlist.";
    }
}