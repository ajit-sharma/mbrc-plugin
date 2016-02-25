namespace MusicBeeRemoteCore.AndroidRemote.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;

    using Fleck;

    using MusicBeeRemoteCore.AndroidRemote.Entities;
    using MusicBeeRemoteCore.AndroidRemote.Events;
    using MusicBeeRemoteCore.AndroidRemote.Persistence;

    using Newtonsoft.Json.Linq;

    using NLog;

    using LogLevel = Fleck.LogLevel;

    /// <summary>
    ///     Wrapper class for the websocket functionality
    /// </summary>
    public sealed class SocketServer : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<IWebSocketConnection> _allSockets;

        private readonly PersistenceController _controller;

        private readonly EventBus bus;

        private bool _isRunning;

        private WebSocketServer _server;

        /// <summary>
        /// </summary>
        public SocketServer(PersistenceController controller, EventBus bus)
        {
            this._controller = controller;
            this.bus = bus;
            this.IsRunning = false;
            this._allSockets = new List<IWebSocketConnection>();
            SetupLogger();
        }

        /// <summary>
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this._isRunning;
            }

            private set
            {
                this._isRunning = value;
                this.bus.Publish(new MessageEvent(MessageEvent.SocketStatusChange, this._isRunning));
            }
        }

        /// <summary>
        ///     Disposes anything Related to the socket _server at the end of life of the Object.
        /// </summary>
        public void Dispose()
        {
            this._server.Dispose();
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
            this.Stop();
            this.Start();
        }

        /// <summary>
        ///     Sends the specified message to all the available clients
        /// </summary>
        /// <param name="message">The message.</param>
        public void Send(string message)
        {
            foreach (var connection in this._allSockets)
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
                if (this._server == null)
                {
                    this._server = new WebSocketServer($"ws://0.0.0.0:{this._controller.Settings.WebSocketPort}");
                    this._server.Start(
                        socket =>
                            {
                                socket.OnOpen = () =>
                                    {
                                        Logger.Debug($"New client connected: {socket.ConnectionInfo.ClientIpAddress}");
                                        this._allSockets.Add(socket);
                                    };

                                socket.OnClose =
                                    () =>
                                        {
                                            Logger.Debug(
                                                $"Client has been disconnected: {socket.ConnectionInfo.ClientIpAddress}");
                                        };

                                socket.OnMessage = message =>
                                    {
                                        Logger.Debug($"New message received: {message}");
                                        var notification = new NotificationMessage(JObject.Parse(message));
                                        this.bus.Publish(new MessageEvent(notification.Message));
                                    };
                            });
                }

                this.IsRunning = true;
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
            if (this._server == null)
            {
                return;
            }

            this._server.Dispose();
            this._server = null;
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