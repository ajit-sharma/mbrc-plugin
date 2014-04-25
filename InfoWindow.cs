using System.Globalization;
using MusicBeePlugin.AndroidRemote.Networking;

namespace MusicBeePlugin
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using AndroidRemote.Settings;
    using Tools;

    /// <summary>
    /// Represents the Settings and monitoring dialog of the plugin.
    /// </summary>
    public partial class InfoWindow : Form
    {

        private BindingList<String> _ipAddressBinding;  
        /// <summary>
        /// 
        /// </summary>
        public InfoWindow()
        {
            InitializeComponent();
            _ipAddressBinding = new BindingList<string>();
        }

        /// <summary>
        /// Updates the visual indicator with the current Socket server status.
        /// </summary>
        /// <param name="isRunning"></param>
        public void UpdateSocketStatus(bool isRunning)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => UpdateSocketStatus(isRunning)));
                return;
            }
            if (isRunning)
            {
                statusLabel.Text = @"Running";
                statusLabel.ForeColor = Color.Green;
            }
            else
            {
                statusLabel.Text = @"Stopped";
                statusLabel.ForeColor = Color.Red;
            }
        }

        private void InfoWindowLoad(object sender, EventArgs e)
        {
            internalIPList.DataSource = NetworkTools.GetPrivateAddressList();
            versionLabel.Text = UserSettings.Instance.CurrentVersion;
            portNumericUpDown.Value = UserSettings.Instance.ListeningPort;
            UpdateFilteringSelection(UserSettings.Instance.FilterSelection);
            UpdateSocketStatus(SocketServer.Instance.IsRunning);
            allowedAddressesComboBox.DataSource = _ipAddressBinding;
        }

        private void SelectionFilteringComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (selectionFilteringComboBox.SelectedIndex)
            {
                case 0:
                    addressLabel.Enabled = false;
                    ipAddressInputTextBox.Enabled = false;
                    rangeNumericUpDown.Enabled = false;
                    addAddressButton.Enabled = false;
                    removeAddressButton.Enabled = false;
                    allowedAddressesComboBox.Enabled = false;
                    allowedLabel.Enabled = false;
                    UserSettings.Instance.FilterSelection = FilteringSelection.All;
                    break;
                case 1:
                    addressLabel.Enabled = true;
                    ipAddressInputTextBox.Enabled = true;
                    rangeNumericUpDown.Enabled = true;
                    addAddressButton.Enabled = false;
                    removeAddressButton.Enabled = false;
                    allowedAddressesComboBox.Enabled = false;
                    allowedLabel.Enabled = false;
                    UserSettings.Instance.FilterSelection = FilteringSelection.Range;
                    break;
                case 2:
                    addressLabel.Enabled = true;
                    ipAddressInputTextBox.Enabled = true;
                    rangeNumericUpDown.Enabled = false;
                    addAddressButton.Enabled = true;
                    removeAddressButton.Enabled = true;
                    allowedAddressesComboBox.Enabled = true;
                    allowedLabel.Enabled = true;
                    UserSettings.Instance.FilterSelection = FilteringSelection.Specific;
                    break;
            }
        }

        private void UpdateFilteringSelection(FilteringSelection selection)
        {
            switch (selection)
            {
                case FilteringSelection.All:
                    selectionFilteringComboBox.SelectedIndex = 0;
                    break;
                case FilteringSelection.Range:
                    ipAddressInputTextBox.Text = UserSettings.Instance.BaseIp;
                    rangeNumericUpDown.Value = UserSettings.Instance.LastOctetMax;
                    selectionFilteringComboBox.SelectedIndex = 1;
                    break;
                case FilteringSelection.Specific:
                    _ipAddressBinding = new BindingList<string>(UserSettings.Instance.IpAddressList);
                    selectionFilteringComboBox.SelectedIndex = 2;
                    break;
                default:
                    selectionFilteringComboBox.SelectedIndex = 0;
                    break;
            }
        }

        private void HandleSaveButtonClick(object sender, EventArgs e)
        {
            UserSettings.Instance.ListeningPort = (uint)portNumericUpDown.Value;
            switch (selectionFilteringComboBox.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    UserSettings.Instance.BaseIp = ipAddressInputTextBox.Text;
                    UserSettings.Instance.LastOctetMax = (uint)rangeNumericUpDown.Value;
                    break;
                case 2:
                    UserSettings.Instance.IpAddressList = new List<string>(_ipAddressBinding);
                    break;

            }
            UserSettings.Instance.SaveSettings();
        }

        private void AddAddressButtonClick(object sender, EventArgs e)
        {
            if (!IsAddressValid()) return;
            if (!_ipAddressBinding.Contains(ipAddressInputTextBox.Text))
            {
                _ipAddressBinding.Add(ipAddressInputTextBox.Text);
            }
        }

        private void RemoveAddressButtonClick(object sender, EventArgs e)
        {
            _ipAddressBinding.Remove(allowedAddressesComboBox.Text);
        }

        private bool IsAddressValid()
        {
            const string pattern = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
            return Regex.IsMatch(ipAddressInputTextBox.Text, pattern);
        }

        private void HandleIpAddressInputTextBoxTextChanged(object sender, EventArgs e)
        {
            var isAddressValid = IsAddressValid();
            ipAddressInputTextBox.BackColor = isAddressValid ? Color.LightGreen : Color.Red;
            if (!isAddressValid || selectionFilteringComboBox.SelectedIndex != 1) return;
            var addressSplit = ipAddressInputTextBox.Text.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            rangeNumericUpDown.Minimum = int.Parse(addressSplit[3]);
        }

        private void InfoWindow_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Process.Start("http://kelsos.net/musicbeeremote/help/");
        }

        /// <summary>
        /// Updates the cache status visual display in the InfoWindow.
        /// </summary>
        /// <param name="covers">The covers cached.</param>
        /// <param name="tracks">The tracks cached.</param>
        public void UpdateCacheStatus(int covers, int tracks)
        {
            cachedCoversValue.Text = covers.ToString(CultureInfo.InvariantCulture);
            cachedTracksValue.Text = tracks.ToString(CultureInfo.InvariantCulture);
        }
    }
}
