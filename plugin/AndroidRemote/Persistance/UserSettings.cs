#region Dependencies

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace MusicBeePlugin.AndroidRemote.Persistance
{
	/// <summary>
	/// Representation of the user settings stored by the remote.
	/// </summary>
	[DataContract(Name = "settings")]
	public class UserSettings
	{
		/// <summary>
		/// Creates a new settings object containing the default values.
		/// </summary>
		public UserSettings()
		{
			HttpPort = 8188;
			WebSocketPort = 8187;
			AllowedAddresses = new List<string>();
			Allowed = MusicBeePlugin.AllowedAddresses.All;
			BaseIp = string.Empty;
			LastOctetMax = 254;
			UpdateFirewallEnabled = true;
		}

		/// <summary>
		/// The port where the HTTP Rest API is available. The plugin receives HTTP
		/// requests and replies at the specific port. Defaults in port 8188
		/// </summary>
		[DataMember(Name = "httpPort")]
		public uint HttpPort { get; set; }

		/// <summary>
		/// The port where the WebSocket server is available. The websocket is
		/// responsible for message passing to the client. The default port is 8187
		/// </summary>
		[DataMember(Name = "webSocketPort")]
		public uint WebSocketPort { get; set; }

		/// <summary>
		/// A list of the addresses allowed to connect to the plugin.
		/// </summary>
		[DataMember(Name = "allowedAddresses")]
		public List<string> AllowedAddresses { get; set; }

		/// <summary>
		/// The type of address filtering active. <see cref="AllowedAddresses"/>.
		/// </summary>
		[DataMember(Name = "filtering")]
		public AllowedAddresses Allowed { get; set; }

		/// <summary>
		/// Used with IP range filtering. It could be something like 192.168.10.x.
		/// </summary>
		[DataMember(Name = "baseIp")]
		public string BaseIp { get; set; }

		/// <summary>
		/// The maximum the last octet in the range filtering can reach. It should
		/// take values from 1-254.
		/// </summary>
		[DataMember(Name = "lastOctet")]
		public uint LastOctetMax { get; set; }

		/// <summary>
		/// If the option is enabled the firewall-utility helper application will run
		/// after the user saves the new settings.
		/// </summary>
		[DataMember(Name = "updateFirewall")]
		public bool UpdateFirewallEnabled { get; set; }

		/// <summary>
		/// The current version of the application.
		/// </summary>
		public string CurrentVersion { get; set; }
	}
}