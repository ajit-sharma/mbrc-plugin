namespace MusicBeeRemote.Core.Settings.Dialog.Commands
{
    public class SaveConfigurationCommand
    {
        private readonly PersistenceManager _manager;

        public SaveConfigurationCommand(PersistenceManager manager)
        {
            _manager = manager;
        }

        public void Execute(object parameter)
        {
            _manager.SaveSettings();
            if (_manager.UserSettingsModel.UpdateFirewallEnabled)
            {
                _manager.UpdateFirewallRules();
            }
        }    
    }
}