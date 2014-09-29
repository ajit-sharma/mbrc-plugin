using ServiceStack.Text;
using System;
using System.IO;

namespace MusicBeePlugin.AndroidRemote.Settings
{
    public class SettingsController
    {
        public UserSettings Settings { get; private set; }
        private readonly string _filename;

        public SettingsController(string storagePath)
        {
            _filename = string.Format("{0}\\settings.json", storagePath);
            Settings = new UserSettings();
        }

        public void SaveSettings()
        {
            var settings = JsonSerializer.SerializeToString(Settings);
            if (_filename == null) return;
            File.WriteAllText(_filename, settings);
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