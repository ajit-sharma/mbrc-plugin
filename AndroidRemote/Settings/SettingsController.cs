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
    }
}