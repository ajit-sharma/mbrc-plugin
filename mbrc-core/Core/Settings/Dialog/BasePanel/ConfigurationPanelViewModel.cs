using System;
using System.Collections.Generic;
using System.Linq;
using MusicBeeRemote.Core.Windows.Mvvm;

namespace MusicBeeRemote.Core.Settings.Dialog.BasePanel
{
    public class ConfigurationPanelViewModel : ViewModelBase
    {
        private readonly UserSettingsModel _userSettings;
        private readonly IVersionProvider _versionProvider;
        

        public ConfigurationPanelViewModel(PersistenceManager persistanceManager,
            IVersionProvider versionProvider)
        {
            _versionProvider = versionProvider;
            _userSettings = persistanceManager.UserSettingsModel;

           
        }

        public void VerifyConnection()
        {
            
        }

        public IEnumerable<FilteringSelection> FilteringData => Enum.GetValues(typeof(FilteringSelection))
            .Cast<FilteringSelection>();

        public FilteringSelection FilteringSelection
        {
            get => _userSettings.FilterSelection;
            set
            {
                _userSettings.FilterSelection = value;
                switch (value)
                {
                    case FilteringSelection.All:
                        _userSettings.IpAddressList = new List<string>();
                        _userSettings.BaseIp = string.Empty;
                        _userSettings.LastOctetMax = 0;
                        break;
                    case FilteringSelection.Range:
                        _userSettings.IpAddressList = new List<string>();
                        break;
                    case FilteringSelection.Specific:
                        _userSettings.BaseIp = string.Empty;
                        _userSettings.LastOctetMax = 0;
                        break;
                }


                OnPropertyChanged(nameof(FilteringSelection));
            }
        }

        public bool DebugEnabled
        {
            get => _userSettings.DebugLogEnabled;
            set
            {
                _userSettings.DebugLogEnabled = value;
                OnPropertyChanged(nameof(DebugEnabled));
            }
        }

        public List<string> LocalIpAddresses => Network.Tools.GetPrivateAddressList();

        public bool FirewallUpdateEnabled
        {
            get => _userSettings.UpdateFirewallEnabled;
            set
            {
                _userSettings.UpdateFirewallEnabled = value;
                OnPropertyChanged(nameof(FirewallUpdateEnabled));
            }
        }

        public uint ListeningPort
        {
            get => _userSettings.ProxyPort;
            set
            {
                _userSettings.ProxyPort = value;
                OnPropertyChanged(nameof(ListeningPort));
            }
        }

        public string PluginVersion => _versionProvider.GetPluginVersion();

        public bool ServiceStatus { get; set; }
    }
}