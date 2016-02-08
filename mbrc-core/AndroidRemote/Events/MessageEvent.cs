#region

using MusicBeePlugin.AndroidRemote.Interfaces;
using System;

#endregion

namespace MusicBeePlugin.AndroidRemote.Events
{
    internal class MessageEvent : IEvent
    {
        public const string ActionClientConnected = "ActionClientConnected";
        public const string ActionClientDisconnected = "ActionClientDisconnected";
        public const string ActionForceClientDisconnect = "ActionForceClientDisconnect";
        public const string ActionDataAvailable = "ActionDataAvailable";
        public const string ActionSocketStart = "ActionSocketStart";
        public const string ActionSocketStop = "ActionSocketStop";
        public const string Notify = "Notify";
        public const string StartServiceBroadcast = "StartServiceBroadcast";
        public const string RestartSocket = "RestartSocket";
        public const string NowPlayingCoverChange = "NowPlayingCoverChange";
        public const string NowPlayingLyricsChange = "NowPlayingLyricsChange";
        public const string SocketStatusChange = "SocketStatusChange";
        public const string ShowFirstRunDialog = "ShowFirstRunDialog";
        private readonly string _clientId;
        private readonly object _data;
        private readonly string _type;


        public MessageEvent(string type, object data)
        {
            _data = data;
            _type = type;
            _clientId = "all";
        }

        public MessageEvent(string type, object data, string clientId)
        {
            _type = type;
            _data = data;
            _clientId = clientId;
        }

        public MessageEvent(string type)
        {
            _type = type;
            _clientId = String.Empty;
            _data = String.Empty;
        }

        public string GetDataString()
        {
            return (string)((_data is string) ? _data : String.Empty);
        }

        public object Data
        {
            get { return _data; }
        }

        public string Type
        {
            get { return _type; }
        }

        public string ClientId
        {
            get { return _clientId; }
        }
    }
}