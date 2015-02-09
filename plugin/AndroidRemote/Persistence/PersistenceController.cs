#region Dependencies

using System;
using System.Diagnostics;
using System.IO;
using ServiceStack.Text;

#endregion

namespace MusicBeePlugin.AndroidRemote.Persistence
{
	/// <summary>
	///     Class responsible for the store and retrieve of Settings and other objects
	///     in the filesystem.
	/// </summary>
	public class PersistenceController
	{
		/// <summary>
		///     The path of the firewall utility.
		/// </summary>
		private readonly string _firewallUtility;

		/// <summary>
		///     The application settings filename.
		/// </summary>
		private readonly string _settingsFilename;

		/// <summary>
		///		The filename of the file that stores dates related to the update of the
		///		cache data. 
		/// </summary>
		private readonly string _dateCacheFileName;

		/// <summary>
		///     Creates a new PersistenceController that stores an retrieves data in the
		///     specified <paramref name="storagePath" />.
		/// </summary>
		/// <param name="storagePath">The path where the files will be stored.</param>
		public PersistenceController(string storagePath)
		{
			_dateCacheFileName = string.Format("{0}\\cache_dates.json", storagePath);
			_settingsFilename = string.Format("{0}\\settings.json", storagePath);
			_firewallUtility = AppDomain.CurrentDomain.BaseDirectory + "\\Plugins\\firewall-utility.exe";
			Settings = new UserSettings();
			DateCache = new LastUpdated();
		}

		/// <summary>
		///     The settings object. Stores the plugin settings during runtime.
		/// </summary>
		public UserSettings Settings { get; private set; }

		/// <summary>
		///		The object stores dates related to the sync of the cache.
		/// </summary>
		public LastUpdated DateCache { get; private set; }

		/// <summary>
		/// Loads the DateCache from the filesystem.
		/// </summary>
		public void LoadDateCache()
		{
			if (!File.Exists(_dateCacheFileName)) return;
			var sr = File.OpenText(_dateCacheFileName);
			var content = sr.ReadToEnd();
			sr.Close();
			if (!string.IsNullOrEmpty(content))
			{
				DateCache = JsonSerializer.DeserializeFromString<LastUpdated>(content);
			}
		}

		/// <summary>
		/// Saves the DateCache to the filesystem.
		/// </summary>
		public void SaveDateCache()
		{
			var dateCache = JsonSerializer.SerializeToString(DateCache);
			if (_dateCacheFileName == null) return;
			File.WriteAllText(_dateCacheFileName, dateCache);
		}

		/// <summary>
		///     Saves the plugin settings to the filesystem.
		/// </summary>
		public void SaveSettings()
		{
			var settings = JsonSerializer.SerializeToString(Settings);
			if (_settingsFilename == null) return;
			File.WriteAllText(_settingsFilename, settings);

			if (Settings.UpdateFirewallEnabled)
			{
				UpdateFirewallRules();
			}
		}

		/// <summary>
		///     When called it will execute the firewall-utility passing the port settings
		///     needed by the plugin.
		/// </summary>
		public void UpdateFirewallRules()
		{
			var startInfo = new ProcessStartInfo(_firewallUtility)
			{
				Verb = "runas",
				Arguments = string.Format("-h {0} -s {1}", Settings.HttpPort, Settings.WebSocketPort)
			};
			Process.Start(startInfo);
		}

		/// <summary>
		///     Loads the settings from the filesystem to the <see cref="UserSettings" />
		///     property to make them available during runtime.
		/// </summary>
		public void LoadSettings()
		{
			if (!File.Exists(_settingsFilename)) return;
			var sr = File.OpenText(_settingsFilename);
			var settings = sr.ReadToEnd();
			sr.Close();
			if (!string.IsNullOrEmpty(settings))
			{
				Settings = JsonSerializer.DeserializeFromString<UserSettings>(settings);
			}
		}
		
		/// <summary>
		///     The method takes an IP address string and checks if it is allowed to connect
		///     based on the application settings
		/// </summary>
		/// <param name="ipString">An IPv4 address.</param>
		/// <returns></returns>
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