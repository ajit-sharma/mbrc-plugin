namespace MusicBeePlugin.Rest.ServiceModel.Const
{
    /// <summary>
    ///     Holds descriptions of various parameters
    /// </summary>
    public class Descriptions
    {
        public const string ActiveOutput = @"Represents the active output of Musicbee";

        public const string Mute = @"If the value is true or false it will mute/unmute the audio." +
                                   "\n If left empty it will toggle mute on/off depending on the previous state.";

        public const string ShuffleState = @"It changes the shuffle state to the specified [off, shuffle, autodj] or goes to the next if null";

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
	    public const string CoverSize = @"The size of the cover";
	    public const string AutoDjPut = @"Enables/disables autodj if set true/false. If left empty it will swap the state.";
	    public const string PlayerAction = @"The action for the player.";
	    public const string NowPlayingPath = "The full path of the track in the filesystem.";
	    public const string NowPlayingId = "The id of the track to be removed from the now playing list.";
	    public const string NowPlayingFrom = "The initial position of the track we want to move in the now playing list.";
	    public const string NowPlayingTo = "The new position where the element will be moved.";
	    public const string MetaType = "The type of meta data the id responds to. Helps identify in which table to lookup.";

	    public const string MoveAction = "The action should be 'now' for clearing queue and immediately " +
	                                     "playing the tracks matching, 'next' for queuing after the current track" +
	                                     ", or 'last' for queue last";

	    public const string NowPlayingQueueId = "The id of the entire entity for which we want to queue tracks.";
	    public const string EntryId = "The id of the entry to retrieve.";
	    public const string TheDateOfTheLastSync = "The date of the last sync.";
	    public const string UpdateChange = "Specifies the type of change requested. Updated/Deleted/Added entries.";
    }
}