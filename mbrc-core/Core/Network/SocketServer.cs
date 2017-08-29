using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Fleck;
using MusicBeeRemote.Core.Settings;
using Newtonsoft.Json.Linq;
using NLog;
using TinyMessenger;
using LogLevel = Fleck.LogLevel;

namespace MusicBeeRemote.Core.Network
{
    /// <summary>
    ///     Wrapper class for the websocket functionality
    /// </summary>
    public sealed class SocketServer : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<IWebSocketConnection> _allSockets;

        private readonly PersistenceManager _manager;

        private readonly ITinyMessengerHub _hub;

        private bool _isRunning;

        private WebSocketServer _server;

        /// <summary>
        /// </summary>
        public SocketServer(PersistenceManager manager, ITinyMessengerHub hub)
        {
            _manager = manager;
            _hub = hub;
            IsRunning = false;
            _allSockets = new List<IWebSocketConnection>();
            SetupLogger();
        }

        /// <summary>
        /// </summary>
        public bool IsRunning
        {
            get => _isRunning;

            private set { _isRunning = value; }
        }

        /// <summary>
        ///     Disposes anything Related to the socket _server at the end of life of the Object.
        /// </summary>
        public void Dispose()
        {
            _server.Dispose();
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId"> </param>
        public void KickClient(string clientId)
        {
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
        ///     It starts the SocketServer.
        /// </summary>
        /// <returns></returns>
        public void Start()
        {
            try
            {
                Logger.Debug("Starting Socket Server");
                if (_server != null)
                {
                    return;
                }

                _server = new WebSocketServer($"ws://0.0.0.0:{_manager.UserSettingsModel.WebSocketPort}");
                _server.Start(
                    socket =>
                    {
                        socket.OnOpen = () =>
                        {
                            Logger.Debug($"New client connected: {socket.ConnectionInfo.ClientIpAddress}");
                            _allSockets.Add(socket);
                        };

                        socket.OnClose =
                            () =>
                            {
                                Logger.Debug(
                                    $"Client has been disconnected: {socket.ConnectionInfo.ClientIpAddress}");
                            };

                        socket.OnMessage = message => { Logger.Debug($"New message received: {message}"); };
                    });

                IsRunning = true;
            }
            catch (SocketException se)
            {
                Logger.Debug(se);
            }
        }

        /// <summary>
        ///     It stops the SocketServer.
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            Logger.Debug("Stopping Socket Server");
            if (_server == null)
            {
                return;
            }

            _server.Dispose();
            _server = null;
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
                        Logger.Debug(ex, message);
                        break;
                    case LogLevel.Error:
                        Logger.Error(ex, message);
                        break;
                    case LogLevel.Info:
                        Logger.Info(ex, message);
                        break;
                    case LogLevel.Warn:
                        Logger.Warn(ex, message);
                        break;
                    default:
                        Logger.Info(ex, message);
                        break;
                }
            };
        }
    }
}