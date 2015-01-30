#region Dependencies

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace MusicBeePlugin.AndroidRemote.Persistance
{
	[DataContract(Name = "settings")]
	public class UserSettings
	{
		public UserSettings()
		{
			HttpPort = 8188;
			Port = 3000;
			AllowedAddresses = new List<string>();
			Allowed = MusicBeePlugin.AllowedAddresses.All;
			BaseIp = String.Empty;
			LastOctetMax = 254;
			UpdateFirewallEnabled = true;
		}

		[DataMember(Name = "http")]
		public uint HttpPort { get; set; }

		[DataMember(Name = "port")]
		public uint Port { get; set; }

		[DataMember(Name = "allowedAddresses")]
		public List<string> AllowedAddresses { get; set; }

		[DataMember(Name = "filtering")]
		public AllowedAddresses Allowed { get; set; }

		[DataMember(Name = "baseIp")]
		public string BaseIp { get; set; }

		[DataMember(Name = "lastOctet")]
		public uint LastOctetMax { get; set; }

		[DataMember(Name = "updateFirewall")]
		public bool UpdateFirewallEnabled { get; set; }

		public string CurrentVersion { get; set; }
	}
}