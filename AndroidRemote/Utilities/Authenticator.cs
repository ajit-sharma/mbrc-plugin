﻿namespace MusicBeePlugin.AndroidRemote.Utilities
{
    using Networking;
    using System.Collections.Concurrent;
    /// <summary>
    /// Responsible for the client authentication. Keeps a list of the connected clients.
    /// </summary>
    public static class Authenticator
    {
        private static readonly ConcurrentDictionary<string, SocketClient> ConnectedClients =
            new ConcurrentDictionary<string, SocketClient>();

        /// <summary>
        ///  Removes a client from the Client List when the client disconnects from the server.
        /// </summary>
        /// <param name="clientId"> </param>
        public static void RemoveClientOnDisconnect(string clientId)
        {
            SocketClient client;
            if (ConnectedClients.TryRemove(clientId, out client))
            {
                //?
            }
        }

        /// <summary>
        /// Adds a client to the Client List when the client connects to the server. In case a client
        /// already exists with the specified clientId then the old client entry is removed before the adding
        /// the new one.
        /// </summary>
        /// <param name="clientId"> </param>
        public static void AddClientOnConnect(string clientId)
        {
            SocketClient client;
            if (ConnectedClients.ContainsKey(clientId))
            {
                ConnectedClients.TryRemove(clientId, out client);
            }
            client = new SocketClient(clientId);
            ConnectedClients.TryAdd(clientId, client);
        }

        /// <summary>
        /// Given a client clientId the function returns a SocketClient object.
        /// </summary>
        /// <param name="clientId">The client clientId.</param>
        /// <returns>A SocketClient object. or null</returns>
        public static SocketClient Client(string clientId)
        {
            SocketClient client;
            ConnectedClients.TryGetValue(clientId, out client);
            return client;
        }
    }
}
