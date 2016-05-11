namespace MusicBeePlugin
{
    partial class InfoWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoWindow));
            this.label1 = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.internalIPList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.rangeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.addAddressButton = new System.Windows.Forms.Button();
            this.allowedLabel = new System.Windows.Forms.Label();
            this.allowedAddressesComboBox = new System.Windows.Forms.ComboBox();
            this.removeAddressButton = new System.Windows.Forms.Button();
            this.addressLabel = new System.Windows.Forms.Label();
            this.ipAddressInputTextBox = new System.Windows.Forms.TextBox();
            this.allowLabel = new System.Windows.Forms.Label();
            this.selectionFilteringComboBox = new System.Windows.Forms.ComboBox();
            this.seperator2 = new System.Windows.Forms.Label();
            this.addressFilteringCategoryLabel = new System.Windows.Forms.Label();
            this.portNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.portLabel = new System.Windows.Forms.Label();
            this.seperator1 = new System.Windows.Forms.Label();
            this.connectionSettingsCategoryLabel = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cached_tracks_label = new System.Windows.Forms.Label();
            this.cached_covers_label = new System.Windows.Forms.Label();
            this.cachedTracksValue = new System.Windows.Forms.Label();
            this.cachedCoversValue = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.httpPortInput = new System.Windows.Forms.NumericUpDown();
            this.updateFirewallRules = new System.Windows.Forms.CheckBox();
            this.qrCodeControl = new Gma.QrCodeNet.Encoding.Windows.Forms.QrCodeImgControl();
            ((System.ComponentModel.ISupportInitialize)(this.rangeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.portNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.httpPortInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.qrCodeControl)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 407);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Plugin Version:";
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(163, 407);
            this.versionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(52, 17);
            this.versionLabel.TabIndex = 1;
            this.versionLabel.Text = "0.0.0.0";
            // 
            // internalIPList
            // 
            this.internalIPList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.internalIPList.FormattingEnabled = true;
            this.internalIPList.ItemHeight = 16;
            this.internalIPList.Location = new System.Drawing.Point(433, 215);
            this.internalIPList.Margin = new System.Windows.Forms.Padding(4);
            this.internalIPList.Name = "internalIPList";
            this.internalIPList.Size = new System.Drawing.Size(158, 146);
            this.internalIPList.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(27, 26);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Status:";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.ForeColor = System.Drawing.Color.Green;
            this.statusLabel.Location = new System.Drawing.Point(88, 26);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(48, 17);
            this.statusLabel.TabIndex = 7;
            this.statusLabel.Text = "Status";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(430, 194);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "Private address list";
            // 
            // rangeNumericUpDown
            // 
            this.rangeNumericUpDown.Location = new System.Drawing.Point(321, 320);
            this.rangeNumericUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.rangeNumericUpDown.Maximum = new decimal(new int[] {
            254,
            0,
            0,
            0});
            this.rangeNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.rangeNumericUpDown.Name = "rangeNumericUpDown";
            this.rangeNumericUpDown.Size = new System.Drawing.Size(57, 22);
            this.rangeNumericUpDown.TabIndex = 28;
            this.rangeNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // addAddressButton
            // 
            this.addAddressButton.Location = new System.Drawing.Point(321, 358);
            this.addAddressButton.Margin = new System.Windows.Forms.Padding(4);
            this.addAddressButton.Name = "addAddressButton";
            this.addAddressButton.Size = new System.Drawing.Size(28, 26);
            this.addAddressButton.TabIndex = 15;
            this.addAddressButton.Text = "+";
            this.addAddressButton.UseVisualStyleBackColor = true;
            this.addAddressButton.Click += new System.EventHandler(this.AddAddressButtonClick);
            // 
            // allowedLabel
            // 
            this.allowedLabel.AutoSize = true;
            this.allowedLabel.Location = new System.Drawing.Point(28, 362);
            this.allowedLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.allowedLabel.Name = "allowedLabel";
            this.allowedLabel.Size = new System.Drawing.Size(60, 17);
            this.allowedLabel.TabIndex = 30;
            this.allowedLabel.Text = "Allowed:";
            // 
            // allowedAddressesComboBox
            // 
            this.allowedAddressesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.allowedAddressesComboBox.FormattingEnabled = true;
            this.allowedAddressesComboBox.Location = new System.Drawing.Point(167, 358);
            this.allowedAddressesComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.allowedAddressesComboBox.Name = "allowedAddressesComboBox";
            this.allowedAddressesComboBox.Size = new System.Drawing.Size(145, 24);
            this.allowedAddressesComboBox.TabIndex = 19;
            // 
            // removeAddressButton
            // 
            this.removeAddressButton.Location = new System.Drawing.Point(351, 358);
            this.removeAddressButton.Margin = new System.Windows.Forms.Padding(4);
            this.removeAddressButton.Name = "removeAddressButton";
            this.removeAddressButton.Size = new System.Drawing.Size(28, 26);
            this.removeAddressButton.TabIndex = 17;
            this.removeAddressButton.Text = "-";
            this.removeAddressButton.UseVisualStyleBackColor = true;
            this.removeAddressButton.Click += new System.EventHandler(this.RemoveAddressButtonClick);
            // 
            // addressLabel
            // 
            this.addressLabel.AutoSize = true;
            this.addressLabel.Location = new System.Drawing.Point(27, 324);
            this.addressLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.addressLabel.Name = "addressLabel";
            this.addressLabel.Size = new System.Drawing.Size(64, 17);
            this.addressLabel.TabIndex = 27;
            this.addressLabel.Text = "Address:";
            // 
            // ipAddressInputTextBox
            // 
            this.ipAddressInputTextBox.Location = new System.Drawing.Point(167, 320);
            this.ipAddressInputTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.ipAddressInputTextBox.Name = "ipAddressInputTextBox";
            this.ipAddressInputTextBox.Size = new System.Drawing.Size(145, 22);
            this.ipAddressInputTextBox.TabIndex = 26;
            this.ipAddressInputTextBox.TextChanged += new System.EventHandler(this.HandleIpAddressInputTextBoxTextChanged);
            // 
            // allowLabel
            // 
            this.allowLabel.AutoSize = true;
            this.allowLabel.Location = new System.Drawing.Point(27, 287);
            this.allowLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.allowLabel.Name = "allowLabel";
            this.allowLabel.Size = new System.Drawing.Size(44, 17);
            this.allowLabel.TabIndex = 25;
            this.allowLabel.Text = "Allow:";
            // 
            // selectionFilteringComboBox
            // 
            this.selectionFilteringComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectionFilteringComboBox.FormattingEnabled = true;
            this.selectionFilteringComboBox.Items.AddRange(new object[] {
            "All",
            "Range",
            "Specified"});
            this.selectionFilteringComboBox.Location = new System.Drawing.Point(167, 283);
            this.selectionFilteringComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.selectionFilteringComboBox.Name = "selectionFilteringComboBox";
            this.selectionFilteringComboBox.Size = new System.Drawing.Size(211, 24);
            this.selectionFilteringComboBox.TabIndex = 24;
            this.selectionFilteringComboBox.SelectedIndexChanged += new System.EventHandler(this.SelectionFilteringComboBoxSelectedIndexChanged);
            // 
            // seperator2
            // 
            this.seperator2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.seperator2.Location = new System.Drawing.Point(25, 266);
            this.seperator2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.seperator2.Name = "seperator2";
            this.seperator2.Size = new System.Drawing.Size(353, 1);
            this.seperator2.TabIndex = 23;
            // 
            // addressFilteringCategoryLabel
            // 
            this.addressFilteringCategoryLabel.AutoSize = true;
            this.addressFilteringCategoryLabel.Location = new System.Drawing.Point(27, 249);
            this.addressFilteringCategoryLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.addressFilteringCategoryLabel.Name = "addressFilteringCategoryLabel";
            this.addressFilteringCategoryLabel.Size = new System.Drawing.Size(112, 17);
            this.addressFilteringCategoryLabel.TabIndex = 22;
            this.addressFilteringCategoryLabel.Text = "Address Allowed";
            // 
            // portNumericUpDown
            // 
            this.portNumericUpDown.Location = new System.Drawing.Point(167, 167);
            this.portNumericUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.portNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.portNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.portNumericUpDown.Name = "portNumericUpDown";
            this.portNumericUpDown.Size = new System.Drawing.Size(212, 22);
            this.portNumericUpDown.TabIndex = 21;
            this.portNumericUpDown.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(27, 170);
            this.portLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(85, 17);
            this.portLabel.TabIndex = 20;
            this.portLabel.Text = "Socket Port:";
            // 
            // seperator1
            // 
            this.seperator1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.seperator1.Location = new System.Drawing.Point(25, 155);
            this.seperator1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.seperator1.Name = "seperator1";
            this.seperator1.Size = new System.Drawing.Size(353, 1);
            this.seperator1.TabIndex = 18;
            // 
            // connectionSettingsCategoryLabel
            // 
            this.connectionSettingsCategoryLabel.AutoSize = true;
            this.connectionSettingsCategoryLabel.Location = new System.Drawing.Point(27, 138);
            this.connectionSettingsCategoryLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.connectionSettingsCategoryLabel.Name = "connectionSettingsCategoryLabel";
            this.connectionSettingsCategoryLabel.Size = new System.Drawing.Size(134, 17);
            this.connectionSettingsCategoryLabel.TabIndex = 16;
            this.connectionSettingsCategoryLabel.Text = "Connection Settings";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(279, 401);
            this.saveButton.Margin = new System.Windows.Forms.Padding(4);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(100, 28);
            this.saveButton.TabIndex = 35;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.HandleSaveButtonClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 62);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 17);
            this.label4.TabIndex = 36;
            this.label4.Text = "Cache";
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(25, 78);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(353, 1);
            this.label5.TabIndex = 37;
            // 
            // cached_tracks_label
            // 
            this.cached_tracks_label.AutoSize = true;
            this.cached_tracks_label.Location = new System.Drawing.Point(28, 91);
            this.cached_tracks_label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.cached_tracks_label.Name = "cached_tracks_label";
            this.cached_tracks_label.Size = new System.Drawing.Size(55, 17);
            this.cached_tracks_label.TabIndex = 38;
            this.cached_tracks_label.Text = "Tracks:";
            // 
            // cached_covers_label
            // 
            this.cached_covers_label.AutoSize = true;
            this.cached_covers_label.Location = new System.Drawing.Point(28, 107);
            this.cached_covers_label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.cached_covers_label.Name = "cached_covers_label";
            this.cached_covers_label.Size = new System.Drawing.Size(56, 17);
            this.cached_covers_label.TabIndex = 39;
            this.cached_covers_label.Text = "Covers:";
            // 
            // cachedTracksValue
            // 
            this.cachedTracksValue.AutoSize = true;
            this.cachedTracksValue.Location = new System.Drawing.Point(163, 91);
            this.cachedTracksValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.cachedTracksValue.Name = "cachedTracksValue";
            this.cachedTracksValue.Size = new System.Drawing.Size(16, 17);
            this.cachedTracksValue.TabIndex = 40;
            this.cachedTracksValue.Text = "0";
            // 
            // cachedCoversValue
            // 
            this.cachedCoversValue.AutoSize = true;
            this.cachedCoversValue.Location = new System.Drawing.Point(163, 107);
            this.cachedCoversValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.cachedCoversValue.Name = "cachedCoversValue";
            this.cachedCoversValue.Size = new System.Drawing.Size(16, 17);
            this.cachedCoversValue.TabIndex = 41;
            this.cachedCoversValue.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 202);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 17);
            this.label6.TabIndex = 42;
            this.label6.Text = "Http Port:";
            // 
            // httpPortInput
            // 
            this.httpPortInput.Location = new System.Drawing.Point(167, 202);
            this.httpPortInput.Margin = new System.Windows.Forms.Padding(4);
            this.httpPortInput.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.httpPortInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.httpPortInput.Name = "httpPortInput";
            this.httpPortInput.Size = new System.Drawing.Size(212, 22);
            this.httpPortInput.TabIndex = 43;
            this.httpPortInput.Value = new decimal(new int[] {
            8188,
            0,
            0,
            0});
            // 
            // updateFirewallRules
            // 
            this.updateFirewallRules.AutoSize = true;
            this.updateFirewallRules.Checked = true;
            this.updateFirewallRules.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updateFirewallRules.Location = new System.Drawing.Point(420, 409);
            this.updateFirewallRules.Margin = new System.Windows.Forms.Padding(4);
            this.updateFirewallRules.Name = "updateFirewallRules";
            this.updateFirewallRules.Size = new System.Drawing.Size(167, 21);
            this.updateFirewallRules.TabIndex = 44;
            this.updateFirewallRules.Text = "Update Firewall Rules";
            this.updateFirewallRules.UseVisualStyleBackColor = true;
            // 
            // qrCodeControl
            // 
            this.qrCodeControl.ErrorCorrectLevel = Gma.QrCodeNet.Encoding.ErrorCorrectionLevel.M;
            this.qrCodeControl.Image = ((System.Drawing.Image)(resources.GetObject("qrCodeControl.Image")));
            this.qrCodeControl.Location = new System.Drawing.Point(433, 26);
            this.qrCodeControl.Name = "qrCodeControl";
            this.qrCodeControl.QuietZoneModule = Gma.QrCodeNet.Encoding.Windows.Render.QuietZoneModules.Two;
            this.qrCodeControl.Size = new System.Drawing.Size(158, 161);
            this.qrCodeControl.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.qrCodeControl.TabIndex = 45;
            this.qrCodeControl.TabStop = false;
            this.qrCodeControl.Text = "qrCodeControl";
            // 
            // InfoWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 438);
            this.Controls.Add(this.qrCodeControl);
            this.Controls.Add(this.updateFirewallRules);
            this.Controls.Add(this.httpPortInput);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cachedCoversValue);
            this.Controls.Add(this.cachedTracksValue);
            this.Controls.Add(this.cached_covers_label);
            this.Controls.Add(this.cached_tracks_label);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.rangeNumericUpDown);
            this.Controls.Add(this.addAddressButton);
            this.Controls.Add(this.allowedLabel);
            this.Controls.Add(this.allowedAddressesComboBox);
            this.Controls.Add(this.removeAddressButton);
            this.Controls.Add(this.addressLabel);
            this.Controls.Add(this.ipAddressInputTextBox);
            this.Controls.Add(this.allowLabel);
            this.Controls.Add(this.selectionFilteringComboBox);
            this.Controls.Add(this.seperator2);
            this.Controls.Add(this.addressFilteringCategoryLabel);
            this.Controls.Add(this.portNumericUpDown);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.seperator1);
            this.Controls.Add(this.connectionSettingsCategoryLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.internalIPList);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InfoWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MusicBee Remote: plugin";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.InfoWindow_HelpButtonClicked);
            this.Load += new System.EventHandler(this.InfoWindowLoad);
            ((System.ComponentModel.ISupportInitialize)(this.rangeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.portNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.httpPortInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.qrCodeControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.ListBox internalIPList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown rangeNumericUpDown;
        private System.Windows.Forms.Button addAddressButton;
        private System.Windows.Forms.Label allowedLabel;
        private System.Windows.Forms.ComboBox allowedAddressesComboBox;
        private System.Windows.Forms.Button removeAddressButton;
        private System.Windows.Forms.Label addressLabel;
        private System.Windows.Forms.TextBox ipAddressInputTextBox;
        private System.Windows.Forms.Label allowLabel;
        private System.Windows.Forms.ComboBox selectionFilteringComboBox;
        private System.Windows.Forms.Label seperator2;
        private System.Windows.Forms.Label addressFilteringCategoryLabel;
        private System.Windows.Forms.NumericUpDown portNumericUpDown;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.Label seperator1;
        private System.Windows.Forms.Label connectionSettingsCategoryLabel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label cached_tracks_label;
        private System.Windows.Forms.Label cached_covers_label;
        private System.Windows.Forms.Label cachedTracksValue;
        private System.Windows.Forms.Label cachedCoversValue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown httpPortInput;
        private System.Windows.Forms.CheckBox updateFirewallRules;
        private Gma.QrCodeNet.Encoding.Windows.Forms.QrCodeImgControl qrCodeControl;
    }
}