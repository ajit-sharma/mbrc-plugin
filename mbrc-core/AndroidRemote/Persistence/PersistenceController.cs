namespace MusicBeePlugin.AndroidRemote.Persistence
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class responsible for the store and retrieve of Settings and other objects
    ///     in the filesystem.
    /// </summary>
    public class PersistenceController
    {
        /// <summary>
        ///     The path of the firewall utility.
        /// </summary>
        private readonly string firewallUtility;

        /// <summary>
        ///     The application settings filename.
        /// </summary>
        private readonly string settingsFilename;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceController"/> class. 
        ///     specified <paramref name="storagePath"/>.
        /// </summary>
        /// <param name="storagePath">
        /// The path where the files will be stored.
        /// </param>
        public PersistenceController(string storagePath)
        {
            this.settingsFilename = $"{storagePath}\\settings.json";
            this.firewallUtility = AppDomain.CurrentDomain.BaseDirectory + "\\Plugins\\firewall-utility.exe";
            this.Settings = new UserSettings();
        }

        /// <summary>
        ///     The settings object. Stores the plugin settings during runtime.
        /// </summary>
        public UserSettings Settings { get; private set; }

        /// <summary>
        ///     The method takes an IP address string and checks if it is allowed to connect
        ///     based on the application settings
        /// </summary>
        /// <param name="ipString">An IPv4 address.</param>
        /// <returns></returns>
        public bool CheckIfAddressIsAllowed(string ipString)
        {
            var isAllowed = false;

            switch (this.Settings.Allowed)
            {
                case AllowedAddresses.Specific:
                    foreach (var source in this.Settings.AllowedAddresses)
                    {
                        if (string.Compare(ipString, source, StringComparison.Ordinal) == 0)
                        {
                            isAllowed = true;
                        }
                    }

                    break;
                case AllowedAddresses.Range:
                    var connectingAddress = ipString.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var baseIp = this.Settings.BaseIp.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (connectingAddress[0] == baseIp[0] && connectingAddress[1] == baseIp[1]
                        && connectingAddress[2] == baseIp[2])
                    {
                        int connectingAddressLowOctet;
                        int baseIpAddressLowOctet;
                        int.TryParse(connectingAddress[3], out connectingAddressLowOctet);
                        int.TryParse(baseIp[3], out baseIpAddressLowOctet);
                        if (connectingAddressLowOctet >= baseIpAddressLowOctet
                            && baseIpAddressLowOctet <= this.Settings.LastOctetMax)
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

        /// <summary>
        ///     Loads the settings from the filesystem to the <see cref="UserSettings" />
        ///     property to make them available during runtime.
        /// </summary>
        public void LoadSettings()
        {
            if (!File.Exists(this.settingsFilename))
            {
                return;
            }

            var sr = File.OpenText(this.settingsFilename);
            var settings = sr.ReadToEnd();
            sr.Close();
            if (!string.IsNullOrEmpty(settings))
            {
                this.Settings = JsonConvert.DeserializeObject<UserSettings>(settings);
            }
        }

        /// <summary>
        ///     Saves the plugin settings to the filesystem.
        /// </summary>
        public void SaveSettings()
        {
            var settings = JsonConvert.SerializeObject(this.Settings);
            if (this.settingsFilename == null)
            {
                return;
            }

            File.WriteAllText(this.settingsFilename, settings);

            if (this.Settings.UpdateFirewallEnabled)
            {
                this.UpdateFirewallRules();
            }
        }

        /// <summary>
        ///     When called it will execute the firewall-utility passing the port settings
        ///     needed by the plugin.
        /// </summary>
        public void UpdateFirewallRules()
        {
            var startInfo = new ProcessStartInfo(this.firewallUtility)
                                {
                                    Verb = "runas", 
                                    Arguments =
                                        $"-h {this.Settings.HttpPort} -s {this.Settings.WebSocketPort}"
                                };
            Process.Start(startInfo);
        }
    }
}