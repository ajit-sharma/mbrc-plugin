namespace MusicBeePlugin.AndroidRemote.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using Settings;
    using Tools;
    using ServiceStack.Text;
    internal class ServiceDiscovery
    {
        private const int MPort = 45345;
        private const string MulticastIp = "239.1.5.10";
        private static readonly IPAddress MulticastAddress = IPAddress.Parse(MulticastIp);
        private static readonly ServiceDiscovery Disc = new ServiceDiscovery();
        private UdpClient _mListener;

        private ServiceDiscovery()
        {
        }

        public static ServiceDiscovery Instance
        {
            get { return Disc; }
        }

        public void Start()
        {
            _mListener = new UdpClient(MPort, AddressFamily.InterNetwork) {EnableBroadcast = true};
            _mListener.JoinMulticastGroup(MulticastAddress);
            _mListener.BeginReceive(OnDataReceived, null);
        }

        private void OnDataReceived(IAsyncResult ar)
        {
            var mEndPoint = new IPEndPoint(IPAddress.Any, MPort);
            var request = _mListener.EndReceive(ar, ref mEndPoint);
            var mRequest = Encoding.UTF8.GetString(request);
            var incoming = JsonObject.Parse(mRequest);

            if (incoming.Get("Message").Contains("discovery"))
            {
                var addresses = NetworkTools.GetPrivateAddressList();
                var clientAddress = IPAddress.Parse(incoming.Get("address"));
                var interfaceAddress = String.Empty;
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

                var notify = new Dictionary<string, object>
                {
                    {"Message", "notify"},
                    {"address", interfaceAddress},
                    {"name", Environment.GetEnvironmentVariable("COMPUTERNAME")},
                    {"port", UserSettings.Instance.ListeningPort}
                };
                var response = Encoding.UTF8.GetBytes(JsonSerializer.SerializeToString(notify));
                _mListener.Send(response, response.Length, mEndPoint);
            }
            _mListener.BeginReceive(OnDataReceived, null);
        }
    }
}
