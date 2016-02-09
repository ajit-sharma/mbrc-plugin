namespace MusicBeePlugin.AndroidRemote.Events
{
    using System;

    using MusicBeePlugin.AndroidRemote.Interfaces;

    public class MessageEvent : IEvent
    {
        public const string ActionClientConnected = "ActionClientConnected";

        public const string ActionClientDisconnected = "ActionClientDisconnected";

        public const string ActionDataAvailable = "ActionDataAvailable";

        public const string ActionForceClientDisconnect = "ActionForceClientDisconnect";

        public const string ActionSocketStart = "ActionSocketStart";

        public const string ActionSocketStop = "ActionSocketStop";

        public const string Notify = "Notify";

        public const string NowPlayingCoverChange = "NowPlayingCoverChange";

        public const string NowPlayingLyricsChange = "NowPlayingLyricsChange";

        public const string RestartSocket = "RestartSocket";

        public const string ShowFirstRunDialog = "ShowFirstRunDialog";

        public const string SocketStatusChange = "SocketStatusChange";

        public const string StartServiceBroadcast = "StartServiceBroadcast";

        private readonly string _clientId;

        private readonly object _data;

        private readonly string _type;

        public MessageEvent(string type, object data)
        {
            this._data = data;
            this._type = type;
            this._clientId = "all";
        }

        public MessageEvent(string type, object data, string clientId)
        {
            this._type = type;
            this._data = data;
            this._clientId = clientId;
        }

        public MessageEvent(string type)
        {
            this._type = type;
            this._clientId = string.Empty;
            this._data = string.Empty;
        }

        public string ClientId
        {
            get
            {
                return this._clientId;
            }
        }

        public object Data
        {
            get
            {
                return this._data;
            }
        }

        public string Type
        {
            get
            {
                return this._type;
            }
        }

        public string GetDataString()
        {
            return (string)((this._data is string) ? this._data : string.Empty);
        }
    }
}