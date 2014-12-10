using System;
using System.Collections.Generic;
using NetFwTypeLib;

namespace firewall_utility
{
    /// <summary>
    ///     Firewall Utility
    /// </summary>
    internal class Program
    {
        /// <summary>
        ///     This is the socket port argument identifier.
        /// </summary>
        private const String Socket = "-s";

        /// <summary>
        ///     This is the HTTP port argument identifier.
        /// </summary>
        private const String Http = "-h";

        /// <summary>
        ///     The name of the HTTP rule.
        /// </summary>
        private const String HttpRule = "MusicBee Remote HTTP REST Server";

        /// <summary>
        ///     The name of the socket rule.
        /// </summary>
        private const String SocketRule = "MusicBee Remote Socket Server";

        /// <summary>
        ///     The Main function of the <see cref="Console" /> application
        /// </summary>
        /// <param name="args">The arguments array</param>
        private static void Main(string[] args)
        {
            var dictionary = new Dictionary<string, int>();

            if (args.Length == 4)
            {
                for (var i = 0; i < args.Length; i += 2)
                {
                    var key = args[i];
                    int val;
                    int.TryParse(args[i + 1], out val);
                    dictionary.Add(key, val);
                }

                int httpPort;
                int socketPort;
                if (dictionary.TryGetValue(Http, out httpPort) && dictionary.TryGetValue(Socket, out socketPort))
                {
                    CreateFirewallRuleForPort(socketPort, SocketRule);
                    CreateFirewallRuleForPort(httpPort, HttpRule);
                    return;
                }
            }

            Console.WriteLine("{0} -s 3000 -h 8188\n", AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine("\t -s: \t This will create the rule for the socket server port");
            Console.WriteLine("\t -h: \t This will create the rule for the HTTP REST server port\n");
            Console.WriteLine("Both arguments are required");
            Console.WriteLine("**For the rules to be created administrative rights are required**");
        }

        /// <summary>
        ///     Creates a firewall rule.
        /// </summary>
        /// <param name="portNumber">The port allowed through the firewall</param>
        /// <param name="ruleName">The name of the newly created rule</param>
        private static void CreateFirewallRuleForPort(int portNumber, string ruleName)
        {
            try
            {
                var fwManagerType = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
                var mgr = (INetFwMgr) Activator.CreateInstance(fwManagerType);
                var firewallEnabled = mgr.LocalPolicy.CurrentProfile.FirewallEnabled;

                if (!firewallEnabled) return;

                var policyType = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
                var firewallPolicy = (INetFwPolicy2) Activator.CreateInstance(policyType);
                var portSt = portNumber.ToString();
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
            catch (Exception)
            {
                Console.WriteLine("The application requires administrative rights. Please run as administrator.");
            }
        }
    }
}