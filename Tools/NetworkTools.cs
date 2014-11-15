using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NetFwTypeLib;
using NLog;

namespace MusicBeePlugin.Tools
{
    class NetworkTools
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Gets a list of the IP Addresses of the network interfaces on the host machine.
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
        /// Given an IP Address it returns it's subnet mask.
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
            throw new ArgumentException(string.Format("unable to find subnet mask for '{0}'", address));
        }

        /// <summary>
        /// Gets the network address.
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
                broadcastBytes[i] = (byte) (addressBytes[i] & maskBytes[i]);
            }
            return new IPAddress(broadcastBytes);
        }

        public static void CreateFirewallRuleForPort(int portNumber)
        {
            try
            {
                const string ruleName = "MusicBee REST Server";
                var fwManagerType = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
                var mgr = (INetFwMgr) Activator.CreateInstance(fwManagerType);
                var firewallEnabled = mgr.LocalPolicy.CurrentProfile.FirewallEnabled;

                if (!firewallEnabled) return;

                var policyType = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
                var firewallPolicy = (INetFwPolicy2) Activator.CreateInstance(policyType);
                var portSt = portNumber.ToString(CultureInfo.InvariantCulture);
                var ruleType = Type.GetTypeFromProgID("HNetCfg.FwRule");
                var firewallRule = (INetFwRule) Activator.CreateInstance(ruleType);
                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                firewallRule.Name = ruleName;
                firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                firewallRule.Enabled = true;
                firewallRule.Protocol = (int) NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                firewallRule.LocalPorts = portSt;
                firewallRule.RemotePorts = portSt;
                firewallRule.InterfaceTypes = "All";

                firewallPolicy.Rules.Add(firewallRule);
            }
            catch (Exception e)
            {
                Logger.DebugException("Firewall Rule Creation", e);
            }
            
        }


    }
}
