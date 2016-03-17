namespace MusicBeeRemoteCore.AndroidRemote.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    using MusicBeeRemoteCore.AndroidRemote.Persistence;
    using MusicBeeRemoteCore.Tools;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using NLog;

    /// <summary>
    ///     UDP Multicast Server class responsible used to provide easy connectivity
    ///     info to the clients in the local network. Used for the automatic detection
    ///     of the server.
    /// </summary>
    internal class ServiceDiscovery
    {

        private ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     The IP address for the UDP multicast. The client has to connect to the
        ///     specified IP address.
        /// </summary>
        private const string MulticastIp = "239.1.5.10";

        /// <summary>
        ///     The port of for the UDP multicast. The client has to connect to the
        ///     specified port.
        /// </summary>
        private const int Port = 45345;

        /// <summary>
        ///     The multicast <see cref="IPAddress" />.
        /// </summary>
        private static readonly IPAddress MulticastAddress = IPAddress.Parse(MulticastIp);

        /// <summary>
        ///     Persistence controller instance responsible for the application settings.
        /// </summary>
        private readonly PersistenceController _controller;

        /// <summary>
        ///     UDP connection listener
        /// </summary>
        private UdpClient _mListener;

        /// <summary>
        ///     Creates a new ServiceDiscover with the provided <see cref="_controller" />.
        /// </summary>
        /// <param name="controller">
        ///     <see cref="_controller" />
        /// </param>
        public ServiceDiscovery(PersistenceController controller)
        {
            this._controller = controller;
        }

        /// <summary>
        ///     Starts listening for incoming UDP multicast connections.
        /// </summary>
        public void Start()
        {
            this.Logger.Debug("Starting Multicast listener");
            this._mListener = new UdpClient(Port, AddressFamily.InterNetwork) { EnableBroadcast = true };
            this._mListener.JoinMulticastGroup(MulticastAddress);
            this._mListener.BeginReceive(this.OnDataReceived, null);
        }

        /// <summary>
        ///     Handles asynchronously any data send through the multicast channel.
        /// </summary>
        /// <param name="ar"></param>
        private void OnDataReceived(IAsyncResult ar)
        {
            var endPoint = new IPEndPoint(IPAddress.Any, Port);
            var request = this._mListener.EndReceive(ar, ref endPoint);
            var requestPayload = Encoding.UTF8.GetString(request);
            var incoming = JObject.Parse(requestPayload);

            this.Logger.Debug($"Incoming multicast message {requestPayload}");

            var token = incoming["context"];
            if (token.ToString().Contains("discovery"))
            {
                var addresses = NetworkTools.GetPrivateAddressList();
                var clientAddress = IPAddress.Parse((string)incoming["address"]);
                var interfaceAddress = string.Empty;
                foreach (var ifAddress in from address in addresses
                                          let ifAddress = IPAddress.Parse(address)
                                          let subnetMask = NetworkTools.GetSubnetMask(address)
                                          let firstNetwork = NetworkTools.GetNetworkAddress(ifAddress, subnetMask)
                                          let secondNetwork = NetworkTools.GetNetworkAddress(clientAddress, subnetMask)
                                          where firstNetwork.Equals(secondNetwork)
                                          select ifAddress)
                {
                    interfaceAddress = ifAddress.ToString();
                    break;
                }

                dynamic notify = new ExpandoObject();
                notify.context = "notify";
                notify.address = interfaceAddress;
                notify.name = Environment.GetEnvironmentVariable("COMPUTERNAME");
                notify.port = this._controller.Settings.WebSocketPort;
                notify.http = this._controller.Settings.HttpPort;

                var serialized = JsonConvert.SerializeObject(notify);
                var response = Encoding.UTF8.GetBytes(serialized);
                this.Logger.Debug($"Sending multicast response {serialized}");
                this._mListener.Send(response, response.Length, endPoint);
            }

            this._mListener.BeginReceive(this.OnDataReceived, null);
        }
    }
}