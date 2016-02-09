namespace MusicBeePlugin.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    using NLog;

    public class NetworkTools
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Gets the network address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="subnetMask">The subnet mask.</param>
        /// <returns>IPAddress.</returns>
        /// <exception cref="ArgumentException">ip and mask lengths don't match</exception>
        public static IPAddress GetNetworkAddress(IPAddress address, IPAddress subnetMask)
        {
            var addressBytes = address.GetAddressBytes();
            var maskBytes = subnetMask.GetAddressBytes();

            if (addressBytes.Length != maskBytes.Length)
            {
                throw new ArgumentException("ip and mask lengths don't match");
            }

            var broadcastBytes = new byte[addressBytes.Length];
            for (var i = 0; i < broadcastBytes.Length; i++)
            {
                broadcastBytes[i] = (byte)(addressBytes[i] & maskBytes[i]);
            }

            return new IPAddress(broadcastBytes);
        }

        /// <summary>
        ///     Gets a list of the IP Addresses of the network interfaces on the host machine.
        /// </summary>
        /// <returns>List{System.String}.</returns>
        public static List<string> GetPrivateAddressList()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return
                (from address in host.AddressList
                 where address.AddressFamily == AddressFamily.InterNetwork
                 select address.ToString()).ToList();
        }

        /// <summary>
        ///     Given an IP Address it returns it's subnet mask.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns>IPAddress.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static IPAddress GetSubnetMask(string ipAddress)
        {
            var address = IPAddress.Parse(ipAddress);
            foreach (var information in from adapter in NetworkInterface.GetAllNetworkInterfaces()
                                        from information in adapter.GetIPProperties().UnicastAddresses
                                        where information.Address.AddressFamily == AddressFamily.InterNetwork
                                        where address.Equals(information.Address)
                                        select information)
            {
                return information.IPv4Mask;
            }

            throw new ArgumentException($"unable to find subnet mask for '{address}'");
        }
    }
}