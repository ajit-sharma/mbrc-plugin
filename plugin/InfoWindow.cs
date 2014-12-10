#region

using MusicBeePlugin.AndroidRemote.Settings;
using MusicBeePlugin.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#endregion

namespace MusicBeePlugin
{
    /// <summary>
    ///     Represents the Settings and monitoring dialog of the plugin.
    /// </summary>
    public partial class InfoWindow : Form
    {
        private readonly SettingsController _controller;

        private BindingList<String> _ipAddressBinding;

        public InfoWindow(SettingsController controller)
        {
            _controller = controller;
            InitializeComponent();
            _ipAddressBinding = new BindingList<string>();
        }

        /// <summary>
        ///     Updates the visual indicator with the current Socket server status.
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
            versionLabel.Text = _controller.Settings.CurrentVersion;
            portNumericUpDown.Value = _controller.Settings.Port;
            httpPortInput.Value = _controller.Settings.HttpPort;
            UpdateFilteringSelection(_controller.Settings.Allowed);
            //UpdateSocketStatus(SocketServer.Instance.IsRunning);
            allowedAddressesComboBox.DataSource = _ipAddressBinding;
            updateFirewallRules.Checked = _controller.Settings.UpdateFirewallEnabled;
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
                    _controller.Settings.Allowed = AllowedAddresses.All;
                    break;
                case 1:
                    addressLabel.Enabled = true;
                    ipAddressInputTextBox.Enabled = true;
                    rangeNumericUpDown.Enabled = true;
                    addAddressButton.Enabled = false;
                    removeAddressButton.Enabled = false;
                    allowedAddressesComboBox.Enabled = false;
                    allowedLabel.Enabled = false;
                    _controller.Settings.Allowed = AllowedAddresses.Range;
                    break;
                case 2:
                    addressLabel.Enabled = true;
                    ipAddressInputTextBox.Enabled = true;
                    rangeNumericUpDown.Enabled = false;
                    addAddressButton.Enabled = true;
                    removeAddressButton.Enabled = true;
                    allowedAddressesComboBox.Enabled = true;
                    allowedLabel.Enabled = true;
                    _controller.Settings.Allowed = AllowedAddresses.Specific;
                    break;
            }
        }

        private void UpdateFilteringSelection(AllowedAddresses selection)
        {
            switch (selection)
            {
                case AllowedAddresses.All:
                    selectionFilteringComboBox.SelectedIndex = 0;
                    break;
                case AllowedAddresses.Range:
                    ipAddressInputTextBox.Text = _controller.Settings.BaseIp;
                    rangeNumericUpDown.Value = _controller.Settings.LastOctetMax;
                    selectionFilteringComboBox.SelectedIndex = 1;
                    break;
                case AllowedAddresses.Specific:
                    _ipAddressBinding = new BindingList<string>(_controller.Settings.AllowedAddresses);
                    selectionFilteringComboBox.SelectedIndex = 2;
                    break;
                default:
                    selectionFilteringComboBox.SelectedIndex = 0;
                    break;
            }
        }

        private void HandleSaveButtonClick(object sender, EventArgs e)
        {
            _controller.Settings.Port = (uint)portNumericUpDown.Value;
            _controller.Settings.UpdateFirewallEnabled = updateFirewallRules.Checked;
            switch (selectionFilteringComboBox.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    _controller.Settings.BaseIp = ipAddressInputTextBox.Text;
                    _controller.Settings.LastOctetMax = (uint)rangeNumericUpDown.Value;
                    break;
                case 2:
                    _controller.Settings.AllowedAddresses = new List<string>(_ipAddressBinding);
                    break;
            }
            _controller.SaveSettings();
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
            const string pattern =
                @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
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
        ///     Updates the cache status visual display in the InfoWindow.
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