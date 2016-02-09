namespace MusicBeePlugin
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using MusicBeePlugin.AndroidRemote.Persistence;
    using MusicBeePlugin.Tools;

    /// <summary>
    ///     Represents the Settings and monitoring dialog of the plugin.
    /// </summary>
    public partial class InfoWindow : Form
    {
        private readonly PersistenceController _controller;

        private BindingList<string> _ipAddressBinding;

        public InfoWindow(PersistenceController controller)
        {
            this._controller = controller;
            this.InitializeComponent();
            this._ipAddressBinding = new BindingList<string>();
        }

        /// <summary>
        ///     Updates the cache status visual display in the InfoWindow.
        /// </summary>
        /// <param name="covers">The covers cached.</param>
        /// <param name="tracks">The tracks cached.</param>
        public void UpdateCacheStatus(int covers, int tracks)
        {
            this.cachedCoversValue.Text = covers.ToString(CultureInfo.InvariantCulture);
            this.cachedTracksValue.Text = tracks.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Updates the visual indicator with the current Socket server status.
        /// </summary>
        /// <param name="isRunning"></param>
        public void UpdateSocketStatus(bool isRunning)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.UpdateSocketStatus(isRunning)));
                return;
            }

            if (isRunning)
            {
                this.statusLabel.Text = @"Running";
                this.statusLabel.ForeColor = Color.Green;
            }
            else
            {
                this.statusLabel.Text = @"Stopped";
                this.statusLabel.ForeColor = Color.Red;
            }
        }

        private void AddAddressButtonClick(object sender, EventArgs e)
        {
            if (!this.IsAddressValid())
            {
                return;
            }

            if (!this._ipAddressBinding.Contains(this.ipAddressInputTextBox.Text))
            {
                this._ipAddressBinding.Add(this.ipAddressInputTextBox.Text);
            }
        }

        private void HandleIpAddressInputTextBoxTextChanged(object sender, EventArgs e)
        {
            var isAddressValid = this.IsAddressValid();
            this.ipAddressInputTextBox.BackColor = isAddressValid ? Color.LightGreen : Color.Red;
            if (!isAddressValid || this.selectionFilteringComboBox.SelectedIndex != 1)
            {
                return;
            }

            var addressSplit = this.ipAddressInputTextBox.Text.Split(
                ".".ToCharArray(), 
                StringSplitOptions.RemoveEmptyEntries);
            this.rangeNumericUpDown.Minimum = int.Parse(addressSplit[3]);
        }

        private void HandleSaveButtonClick(object sender, EventArgs e)
        {
            this._controller.Settings.WebSocketPort = (uint)this.portNumericUpDown.Value;
            this._controller.Settings.UpdateFirewallEnabled = this.updateFirewallRules.Checked;
            switch (this.selectionFilteringComboBox.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    this._controller.Settings.BaseIp = this.ipAddressInputTextBox.Text;
                    this._controller.Settings.LastOctetMax = (uint)this.rangeNumericUpDown.Value;
                    break;
                case 2:
                    this._controller.Settings.AllowedAddresses = new List<string>(this._ipAddressBinding);
                    break;
            }

            this._controller.SaveSettings();
        }

        private void InfoWindow_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Process.Start("http://kelsos.net/musicbeeremote/help/");
        }

        private void InfoWindowLoad(object sender, EventArgs e)
        {
            this.internalIPList.DataSource = NetworkTools.GetPrivateAddressList();
            this.versionLabel.Text = this._controller.Settings.CurrentVersion;
            this.portNumericUpDown.Value = this._controller.Settings.WebSocketPort;
            this.httpPortInput.Value = this._controller.Settings.HttpPort;
            this.UpdateFilteringSelection(this._controller.Settings.Allowed);

            // UpdateSocketStatus(SocketServer.Instance.IsRunning);
            this.allowedAddressesComboBox.DataSource = this._ipAddressBinding;
            this.updateFirewallRules.Checked = this._controller.Settings.UpdateFirewallEnabled;
        }

        private bool IsAddressValid()
        {
            const string pattern =
                @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
            return Regex.IsMatch(this.ipAddressInputTextBox.Text, pattern);
        }

        private void RemoveAddressButtonClick(object sender, EventArgs e)
        {
            this._ipAddressBinding.Remove(this.allowedAddressesComboBox.Text);
        }

        private void SelectionFilteringComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.selectionFilteringComboBox.SelectedIndex)
            {
                case 0:
                    this.addressLabel.Enabled = false;
                    this.ipAddressInputTextBox.Enabled = false;
                    this.rangeNumericUpDown.Enabled = false;
                    this.addAddressButton.Enabled = false;
                    this.removeAddressButton.Enabled = false;
                    this.allowedAddressesComboBox.Enabled = false;
                    this.allowedLabel.Enabled = false;
                    this._controller.Settings.Allowed = AllowedAddresses.All;
                    break;
                case 1:
                    this.addressLabel.Enabled = true;
                    this.ipAddressInputTextBox.Enabled = true;
                    this.rangeNumericUpDown.Enabled = true;
                    this.addAddressButton.Enabled = false;
                    this.removeAddressButton.Enabled = false;
                    this.allowedAddressesComboBox.Enabled = false;
                    this.allowedLabel.Enabled = false;
                    this._controller.Settings.Allowed = AllowedAddresses.Range;
                    break;
                case 2:
                    this.addressLabel.Enabled = true;
                    this.ipAddressInputTextBox.Enabled = true;
                    this.rangeNumericUpDown.Enabled = false;
                    this.addAddressButton.Enabled = true;
                    this.removeAddressButton.Enabled = true;
                    this.allowedAddressesComboBox.Enabled = true;
                    this.allowedLabel.Enabled = true;
                    this._controller.Settings.Allowed = AllowedAddresses.Specific;
                    break;
            }
        }

        private void UpdateFilteringSelection(AllowedAddresses selection)
        {
            switch (selection)
            {
                case AllowedAddresses.All:
                    this.selectionFilteringComboBox.SelectedIndex = 0;
                    break;
                case AllowedAddresses.Range:
                    this.ipAddressInputTextBox.Text = this._controller.Settings.BaseIp;
                    this.rangeNumericUpDown.Value = this._controller.Settings.LastOctetMax;
                    this.selectionFilteringComboBox.SelectedIndex = 1;
                    break;
                case AllowedAddresses.Specific:
                    this._ipAddressBinding = new BindingList<string>(this._controller.Settings.AllowedAddresses);
                    this.selectionFilteringComboBox.SelectedIndex = 2;
                    break;
                default:
                    this.selectionFilteringComboBox.SelectedIndex = 0;
                    break;
            }
        }
    }
}