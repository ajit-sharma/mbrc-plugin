namespace MusicBeePlugin.Debugging
{
    partial class DebugTool
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.buildCache = new System.Windows.Forms.Button();
            this.getCoverButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buildCache
            // 
            this.buildCache.Location = new System.Drawing.Point(12, 12);
            this.buildCache.Name = "buildCache";
            this.buildCache.Size = new System.Drawing.Size(75, 23);
            this.buildCache.TabIndex = 2;
            this.buildCache.Text = "Build Cache";
            this.buildCache.UseVisualStyleBackColor = true;
            this.buildCache.Click += new System.EventHandler(this.OnTestButtonClick);
            // 
            // getCoverButton
            // 
            this.getCoverButton.Location = new System.Drawing.Point(12, 41);
            this.getCoverButton.Name = "getCoverButton";
            this.getCoverButton.Size = new System.Drawing.Size(75, 23);
            this.getCoverButton.TabIndex = 11;
            this.getCoverButton.Text = "Get Covers";
            this.getCoverButton.UseVisualStyleBackColor = true;
            this.getCoverButton.Click += new System.EventHandler(this.getCoverButton_Click);
            // 
            // DebugTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 152);
            this.Controls.Add(this.getCoverButton);
            this.Controls.Add(this.buildCache);
            this.Name = "DebugTool";
            this.Text = "DebugTool";
            this.Load += new System.EventHandler(this.DebugTool_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buildCache;
        private System.Windows.Forms.Button getCoverButton;
    }
}