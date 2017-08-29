using System.Collections.Generic;
using System.Runtime.Serialization;
using MusicBeeRemote.Core.Enumerations;

namespace MusicBeeRemote.Core.Settings
{
    /// <summary>
    /// Representation of the user settings stored by the remote.
    /// </summary>
    [DataContract(Name = "settings")]
    public class UserSettingsModel
    {
        /// <summary>
        /// The port of the proxy that serves both the http and websocket connection.
        /// The proxy is used to avoid having the client to handle two distinct ports.
        /// </summary>
        [DataMember(Name = "proxy_port")]
        public uint ProxyPort { get; set; } = 8180;

        /// <summary>
        /// The type of address filtering active. <see cref="IpAddressList"/>.
        /// </summary>
        [DataMember(Name = "filtering")]
        public FilteringSelection FilterSelection { get; set; } = FilteringSelection.All;

        /// <summary>
        /// Used with <see cref="FilteringSelection.Specific"/>. When the specific filtering mode is active
        /// only the IPv4 addresses contained withing this list will be allowed to connect to the plugin.
        /// </summary>
        [DataMember(Name = "specific_ips")]
        public List<string> IpAddressList { get; set; } = new List<string>();

        /// <summary>
        /// Used with <see cref="FilteringSelection.Range"/> filtering to indicate the starting address of an IPv4
        /// range that will be allowed to connect to the plugin.
        /// <example>192.168.1.10</example>
        /// </summary>
        [DataMember(Name = "base_ip")]
        public string BaseIp { get; set; } = "";

        /// <summary>
        /// The current version of the plugin. Peristed and saved in order to ensure that the <see cref="InfoWindow"/>
        /// is shown once after each update to the user.
        /// </summary>
        [DataMember(Name = "plugin_version")]
        public string CurrentVersion { get; set; } = "";

        /// <summary>
        /// The port where the HTTP Rest API is available. The plugin receives HTTP
        /// requests and replies at the specific port. Defaults in port 8188
        /// </summary>
        [DataMember(Name = "http_port")]
        public uint HttpPort { get; set; } = 8187;

        /// <summary>
        /// Used with <see cref="FilteringSelection.Range"/> filtering to indicate the last octet of the IPv4 address.
        /// The allowed addresses range will start with <see cref="BaseIp"/> and will end to the digit indicated
        /// by the last octet.
        /// <example>
        /// BaseIp = 192.168.1.10
        /// LastOctetMax = 20
        ///
        /// This means that the only the addresses from 192.168.1.10 -> 192.168.1.20 will be allowed to connect.
        /// </example>
        /// </summary>
        [DataMember(Name = "last_octet")]
        public uint LastOctetMax { get; set; }

        /// <summary>
        /// If the option is enabled the firewall-utility helper application will run
        /// after the user saves the new settings.
        /// </summary>
        [DataMember(Name = "update_firewall")]
        public bool UpdateFirewallEnabled { get; set; }

        /// <summary>
        /// The port where the WebSocket server is available. The websocket is
        /// responsible for message passing to the client. The default port is 8187
        /// </summary>
        [DataMember(Name = "websocket_port")]
        public uint WebSocketPort { get; set; } = 8188;

        /// <summary>
        /// If this value is true then the fully verbose debug logging is active while on the release build.
        /// Under normal circumstances this should be deactivated.
        /// </summary>
        [DataMember(Name = "debug_logs")]
        public bool DebugLogEnabled { get; set; }
    }
}