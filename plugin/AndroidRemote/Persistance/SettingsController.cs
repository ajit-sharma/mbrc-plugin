#region Dependencies

using System;
using System.Diagnostics;
using System.IO;
using ServiceStack.Text;

#endregion

namespace MusicBeePlugin.AndroidRemote.Persistance
{
	public class SettingsController
	{
		private readonly string _filename;
		private readonly string _fUtil;

		public SettingsController(string storagePath)
		{
			_filename = string.Format("{0}\\settings.json", storagePath);
			_fUtil = AppDomain.CurrentDomain.BaseDirectory + "\\Plugins\\firewall-utility.exe";
			Settings = new UserSettings();
		}

		public UserSettings Settings { get; private set; }

		public void SaveSettings()
		{
			var settings = JsonSerializer.SerializeToString(Settings);
			if (_filename == null) return;
			File.WriteAllText(_filename, settings);

			if (Settings.UpdateFirewallEnabled)
			{
				UpdateFirewallRules();
			}
		}

		public void UpdateFirewallRules()
		{
			var startInfo = new ProcessStartInfo(_fUtil)
			{
				Verb = "runas",
				Arguments = string.Format("-h {0} -s {1}", Settings.HttpPort, Settings.Port)
			};
			Process.Start(startInfo);
		}

		public void LoadSettings()
		{
			if (!File.Exists(_filename)) return;
			var sr = File.OpenText(_filename);
			var settings = sr.ReadToEnd();
			sr.Close();
			if (!String.IsNullOrEmpty(settings))
			{
				Settings = JsonSerializer.DeserializeFromString<UserSettings>(settings);
			}
		}

		public bool CheckIfAddressIsAllowed(string ipString)
		{
			var isAllowed = false;

			switch (Settings.Allowed)
			{
				case AllowedAddresses.Specific:
					foreach (var source in Settings.AllowedAddresses)
					{
						if (string.Compare(ipString, source, StringComparison.Ordinal) == 0)
						{
							isAllowed = true;
						}
					}
					break;
				case AllowedAddresses.Range:
					var connectingAddress = ipString.Split(".".ToCharArray(),
						StringSplitOptions.RemoveEmptyEntries);
					var baseIp = Settings.BaseIp.Split(".".ToCharArray(),
						StringSplitOptions.RemoveEmptyEntries);
					if (connectingAddress[0] == baseIp[0] && connectingAddress[1] == baseIp[1] &&
					    connectingAddress[2] == baseIp[2])
					{
						int connectingAddressLowOctet;
						int baseIpAddressLowOctet;
						int.TryParse(connectingAddress[3], out connectingAddressLowOctet);
						int.TryParse(baseIp[3], out baseIpAddressLowOctet);
						if (connectingAddressLowOctet >= baseIpAddressLowOctet &&
						    baseIpAddressLowOctet <= Settings.LastOctetMax)
						{
							isAllowed = true;
						}
					}
					break;
				default:
					isAllowed = true;
					break;
			}
			return isAllowed;
		}
	}
}