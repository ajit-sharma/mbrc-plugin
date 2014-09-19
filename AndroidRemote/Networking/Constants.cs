namespace MusicBeePlugin.AndroidRemote.Networking
{
    internal static class Constants
    {
        #region Protocol 2. Basic functionality

        public const string Player = "player";
        public const string Protocol = "protocol";
        public const string NotAllowed = "notallowed";

        #endregion Protocol 2. Basic functionality

        #region Protocol 2. API calls

        public const string NowPlayingCover = "nowplayingcover";
        public const string NowPlayingLyrics = "nowplayinglyrics";
        public const string NowPlayingListPlay = "nowplayinglistplay";

        #endregion Protocol 2. API calls


        #region Protocol 2. SocketMessage type identifiers.

        public const string Reply = "rep";
        public const string Message = "msg";

        #endregion Protocol 2. SocketMessage type identifiers.
    }
}