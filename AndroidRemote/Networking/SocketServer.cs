namespace MusicBeePlugin.AndroidRemote.Networking
{
    using NLog;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using Entities;
    using Events;
    using Settings;
    using Utilities;

    /// <summary>
    ///     The socket server.
    /// </summary>
    public sealed class SocketServer : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ProtocolHandler _handler;

        private readonly ConcurrentDictionary<string, Socket> _availableWorkerSockets;

        /// <summary>
        ///     The main socket. This is the Socket that listens for new client connections.
        /// </summary>
        private Socket _mainSocket;

        private static readonly SocketServer Server = new SocketServer();

        /// <summary>
        ///     The worker callback.
        /// </summary>
        private AsyncCallback _workerCallback;

        private bool _isRunning;


        /// <summary>
        /// Gets the instance of the Socket Server singleton
        /// </summary>
        /// <value>The instance.</value>
        public static SocketServer Instance
        {
            get { return Server; }
        }

        /// <summary>
        /// </summary>
        private SocketServer()
        {
            _handler = new ProtocolHandler();
            IsRunning = false;
            _availableWorkerSockets = new ConcurrentDictionary<string, Socket>();
        }

        /// <summary>
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
            private set
            {
                _isRunning = value;
                EventBus.FireEvent(new MessageEvent(EventType.SocketStatusChange, _isRunning));
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="clientId"> </param>
        public void KickClient(string clientId)
        {
            try
            {
                Socket workerSocket;
                if (!_availableWorkerSockets.TryRemove(clientId, out workerSocket)) return;
                workerSocket.Close();
                workerSocket.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);
            }
        }

        /// <summary>
        ///     It stops the SocketServer.
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            Logger.Info("Stopping Socket Server");
            try
            {
                if (_mainSocket != null)
                {
                    _mainSocket.Close();
                }

                foreach (var wSocket in _availableWorkerSockets.Values.Where(wSocket => wSocket != null))
                {
                    wSocket.Close();
                    wSocket.Dispose();
                }
                _mainSocket = null;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);
            }
            finally
            {
                IsRunning = false;
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
                Logger.Info("Starting Socket Server");
                _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // Create the listening socket.    
                var ipLocal = new IPEndPoint(IPAddress.Any, Convert.ToInt32(UserSettings.Instance.ListeningPort));
                // Bind to local IP address.
                _mainSocket.Bind(ipLocal);
                // Start Listening.
                _mainSocket.Listen(4);
                // Create the call back for any client connections.
                _mainSocket.BeginAccept(OnClientConnect, null);
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

        // this is the call back function,
        private void OnClientConnect(IAsyncResult ar)
        {
            try
            {
                // Here we complete/end the BeginAccept asynchronous call
                // by calling EndAccept() - Which returns the reference
                // to a new Socket object.
                var workerSocket = _mainSocket.EndAccept(ar);

                // Validate If client should connect.
                var ipAddress = ((IPEndPoint) workerSocket.RemoteEndPoint).Address;
                var ipString = ipAddress.ToString();

                var isAllowed = false;
                switch (UserSettings.Instance.FilterSelection)
                {
                    case FilteringSelection.Specific:
                        foreach (var source in UserSettings.Instance.IpAddressList)
                        {
                            if (string.Compare(ipString, source, StringComparison.Ordinal) == 0)
                            {
                                isAllowed = true;
                            }
                        }
                        break;
                    case FilteringSelection.Range:
                        var connectingAddress = ipString.Split(".".ToCharArray(),
                            StringSplitOptions.RemoveEmptyEntries);
                        var baseIp = UserSettings.Instance.BaseIp.Split(".".ToCharArray(),
                            StringSplitOptions.RemoveEmptyEntries);
                        if (connectingAddress[0] == baseIp[0] && connectingAddress[1] == baseIp[1] &&
                            connectingAddress[2] == baseIp[2])
                        {
                            int connectingAddressLowOctet;
                            int baseIpAddressLowOctet;
                            int.TryParse(connectingAddress[3], out connectingAddressLowOctet);
                            int.TryParse(baseIp[3], out baseIpAddressLowOctet);
                            if (connectingAddressLowOctet >= baseIpAddressLowOctet &&
                                baseIpAddressLowOctet <= UserSettings.Instance.LastOctetMax)
                            {
                                isAllowed = true;
                            }
                        }
                        break;
                    default:
                        isAllowed = true;
                        break;
                }
                if (!isAllowed)
                {
                    workerSocket.Send(Encoding.UTF8.GetBytes(
                            new SocketMessage(Constants.NotAllowed,
                                Constants.Reply,
                                String.Empty)
                                .toJsonString()));
                    workerSocket.Close();
                    Logger.Debug("Force Disconnected not valid range");
                    _mainSocket.BeginAccept(OnClientConnect, null);
                    return;
                }

                var clientId = IdGenerator.GetUniqueKey();

                if (!_availableWorkerSockets.TryAdd(clientId, workerSocket)) return;
                // Inform the the Protocol Handler that a new Client has been connected, prepare for handshake.
                EventBus.FireEvent(new MessageEvent(EventType.ActionClientConnected, string.Empty, clientId));

                // Let the worker Socket do the further processing 
                // for the just connected client.
                var socketState = new SocketState
                {
                    ClientSocket = workerSocket,
                    ClientId = clientId
                };

                WaitForData(socketState);
            }
            catch (ObjectDisposedException)
            {
                Logger.Debug("OnClientConnection: Socket has been closed");
            }
            catch (SocketException se)
            {
                Logger.Debug(se);
            }
            catch (Exception ex)
            {
                Logger.Debug("OnClientConnect", ex);
            }
            finally
            {
                try
                {
                    // Since the main Socket is now free, it can go back and
                    // wait for the other clients who are attempting to connect
                    _mainSocket.BeginAccept(OnClientConnect, null);
                }
                catch (Exception e)
                {
                    Logger.Debug("OnClientConnect", e);
                }
            }
        }

        // Start waiting for data from the client
        private void WaitForData(SocketState state)
        {
            try
            {
                if (_workerCallback == null)
                {
                    // Specify the call back function which is to be
                    // invoked when there is any write activity by the
                    // connected client.
                    _workerCallback = OnDataReceived;
                }

                state.ClientSocket.BeginReceive(state.DataBuffer, 0, SocketState.BufferSize,
                    SocketFlags.None, _workerCallback, state);
            }
            catch (SocketException se)
            {
                if (se.ErrorCode != 10053)
                {
                    Logger.Debug(se);
                }
                else
                {
                    EventBus.FireEvent(new MessageEvent(EventType.ActionClientDisconnected, string.Empty, state.ClientId));
                }
            }
        }

        // This is the call back function which will be invoked when the socket
        // detects any client writing of data on the stream
        private void OnDataReceived(IAsyncResult ar)
        {
            var clientId = String.Empty;
            try
            {
                var socketState = (SocketState) ar.AsyncState;
                // Complete the BeginReceive() asynchronous call by EndReceive() method
                // which will return the number of characters written to the stream
                // by the client.

                clientId = socketState.ClientId;

                var iRx = socketState.ClientSocket.EndReceive(ar);

                var chars = new char[iRx];

                var decoder = Encoding.UTF8.GetDecoder();

                decoder.GetChars(socketState.DataBuffer, 0, iRx, chars, 0);
                var length = chars.Length;

                if (length == 0)
                {
                    WaitForData(socketState);
                    return;
                }

                socketState.mBuilder.Append(chars);

                var message = socketState.mBuilder.ToString().Replace("\0", "");
                if (!message.Contains(Environment.NewLine))
                {
                    WaitForData(socketState);
                    return;
                }

                var lastIndex = message.LastIndexOf(Environment.NewLine, StringComparison.Ordinal);
                var afterLast = message.Substring(lastIndex + 2);

                var messages =
                    new List<string>(message.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries));

                if (afterLast.Length > 0)
                {
                    var last = messages.Last();
                    messages.Remove(last);
                    socketState.mBuilder.Clear();
                    socketState.mBuilder.Append(last);
                }
                else
                {
                    socketState.mBuilder.Clear();
                }

                if (messages.Count > 0)
                {
                    _handler.ProcessIncomingMessage(messages, clientId);
                }

                // Continue the waiting for data on the Socket.
                WaitForData(socketState);
            }
            catch (ObjectDisposedException)
            {
                EventBus.FireEvent(new MessageEvent(EventType.ActionClientDisconnected, string.Empty, clientId));
                Logger.Debug("OnDataReceived, socket has been closed");
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054) // Error code for Connection reset by peer
                {
                    Socket deadSocket;
                    if (_availableWorkerSockets.ContainsKey(clientId))
                        _availableWorkerSockets.TryRemove(clientId, out deadSocket);
                    EventBus.FireEvent(new MessageEvent(EventType.ActionClientDisconnected, string.Empty, clientId));
                }
                else
                {
                    Logger.Debug(se);
                }
            }
        }

        /// <summary>
        /// Send the message specified to a single client matched by the
        /// client's identity.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="clientId">The client identifier.</param>
        public void Send(string message, string clientId)
        {
            if (clientId.Equals("all"))
            {
                Send(message);
                return;
            }
            try
            {
                var data = Encoding.UTF8.GetBytes(message + "\r\n");
                Socket wSocket;
                if (_availableWorkerSockets.TryGetValue(clientId, out wSocket))
                {
                    wSocket.Send(data);
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("Send", ex);
            }
        }

        /// <summary>
        /// Removes an inactive socket from the client list.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        private void RemoveDeadSocket(string clientId)
        {
            Socket worker;
            _availableWorkerSockets.TryRemove(clientId, out worker);
            if (worker != null)
            {
                worker.Dispose();
            }
        }


        /// <summary>
        /// Sends the specified message to all the available clients
        /// </summary>
        /// <param name="message">The message.</param>
        public void Send(string message)
        {
            try
            {
                var data = Encoding.UTF8.GetBytes(message);

                foreach (var key in _availableWorkerSockets.Keys)
                {
                    Socket worker;
                    if (!_availableWorkerSockets.TryGetValue(key, out worker)) continue;
                    var isConnected = (worker != null && worker.Connected);
                    if (!isConnected)
                    {
                        RemoveDeadSocket(key);
                        EventBus.FireEvent(new MessageEvent(EventType.ActionClientDisconnected, string.Empty, key));
                    }
                    if (isConnected && Authenticator.IsClientAuthenticated(key))
                    {
                        worker.Send(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("Send (Broadcast)", ex);
            }
        }

        /// <summary>
        ///     Disposes anything Related to the socket server at the end of life of the Object.
        /// </summary>
        public void Dispose()
        {
            _mainSocket.Dispose();
        }
    }
}
