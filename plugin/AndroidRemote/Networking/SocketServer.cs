#region

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Fleck;
using MusicBeePlugin.AndroidRemote.Entities;
using MusicBeePlugin.AndroidRemote.Events;
using MusicBeePlugin.AndroidRemote.Persistence;
using NLog;
using ServiceStack.Text;
using LogLevel = Fleck.LogLevel;

#endregion

namespace MusicBeePlugin.AndroidRemote.Networking
{
    /// <summary>
    ///     Wrapper class for the websocket functionality
    /// </summary>
    public sealed class SocketServer : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly List<IWebSocketConnection> _allSockets;
        private readonly PersistenceController _controller;
        private bool _isRunning;
        private WebSocketServer server;

        /// <summary>
        /// </summary>
        public SocketServer(PersistenceController controller)
        {
            _controller = controller;
            IsRunning = false;
            _allSockets = new List<IWebSocketConnection>();
            SetupLogger();
        }

        /// <summary>
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
            private set
            {
                _isRunning = value;
                EventBus.FireEvent(new MessageEvent(MessageEvent.SocketStatusChange, _isRunning));
            }
        }

        /// <summary>
        ///     Disposes anything Related to the socket server at the end of life of the Object.
        /// </summary>
        public void Dispose()
        {
            server.Dispose();
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId"> </param>
        public void KickClient(string clientId)
        {
        }

        /// <summary>
        ///     It stops the SocketServer.
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            Logger.Debug("Stopping Socket Server");
            if (server == null) return;
            server.Dispose();
            server = null;
        }

        /// <summary>
        ///     It starts the SocketServer.
        /// </summary>
        /// <returns></returns>
        public void Start()
        {
            try
            {
                Logger.Debug("Starting Socket Server");
                if (server == null)
                {
                    server = new WebSocketServer(string.Format("ws://0.0.0.0:{0}", _controller.Settings.WebSocketPort));
                    server.Start(socket =>
                    {
                        socket.OnOpen = () =>
                        {
                            Logger.Debug(string.Format("New client connected: {0}",
                                socket.ConnectionInfo.ClientIpAddress));
                            _allSockets.Add(socket);
                        };

                        socket.OnClose = () =>
                        {
                            Logger.Debug(string.Format("Client has been disconnected: {0}",
                                socket.ConnectionInfo.ClientIpAddress));
                        };

                        socket.OnMessage = message =>
                        {
                            Logger.Debug(string.Format("New message received: {0}", message));
                            var notification = new NotificationMessage(JsonObject.Parse(message));
                            EventBus.FireEvent(new MessageEvent(notification.Message));
                        };
                    });
                }

                IsRunning = true;
            }
            catch (SocketException se)
            {
                Logger.Debug(se);
            }
        }

        /// <summary>
        ///     Restarts the main socket that is listening for new clients.
        ///     Useful when the user wants to change the listening port.
        /// </summary>
        public void RestartSocket()
        {
            Stop();
            Start();
        }

        /// <summary>
        ///     Sends the specified message to all the available clients
        /// </summary>
        /// <param name="message">The message.</param>
        public void Send(string message)
        {
            foreach (var connection in _allSockets)
            {
                connection.Send(message);
            }
        }

        /// <summary>
        /// Used to setup fleck in order to use NLOG for the logging functionality.
        /// </summary>
        private static void SetupLogger()
        {
            FleckLog.LogAction = (level, message, ex) =>
            {
                switch (level)
                {
                    case LogLevel.Debug:
                        Logger.Debug(message, ex);
                        break;
                    case LogLevel.Error:
                        Logger.Error(message, ex);
                        break;
                    case LogLevel.Info:
                        Logger.Info(message, ex);
                        break;
                    case LogLevel.Warn:
                        Logger.Warn(message, ex);
                        break;
                    default:
                        Logger.Info(message, ex);
                        break;
                }
            };
        }
    }
}