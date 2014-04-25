using System;
using System.Text;

namespace MusicBeePlugin.AndroidRemote.Networking
{
    using System.Net.Sockets;

    /// <summary>
    /// 
    /// </summary>
    public class SocketState
    {
        public const int BufferSize = 8192;
        /// <summary>
        /// 
        /// </summary>
        public Socket ClientSocket = null;

        /// <summary>
        /// 
        /// </summary>
        public string ClientId = String.Empty;

        // Buffer to store the data sent by the client
        public byte[] DataBuffer = new byte[BufferSize];
        
        public StringBuilder mBuilder = new StringBuilder();
    }
}
