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
            this.artworkTime = new System.Windows.Forms.TextBox();
            this.artistTime = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
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
            // artworkTime
            // 
            this.artworkTime.Location = new System.Drawing.Point(93, 14);
            this.artworkTime.Name = "artworkTime";
            this.artworkTime.ReadOnly = true;
            this.artworkTime.Size = new System.Drawing.Size(100, 20);
            this.artworkTime.TabIndex = 12;
            // 
            // artistTime
            // 
            this.artistTime.Location = new System.Drawing.Point(93, 43);
            this.artistTime.Name = "artistTime";
            this.artistTime.ReadOnly = true;
            this.artistTime.Size = new System.Drawing.Size(100, 20);
            this.artistTime.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(199, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Artwork";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(199, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Artist Artwork";
            // 
            // DebugTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 82);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.artistTime);
            this.Controls.Add(this.artworkTime);
            this.Controls.Add(this.getCoverButton);
            this.Controls.Add(this.buildCache);
            this.Name = "DebugTool";
            this.Text = "DebugTool";
            this.Load += new System.EventHandler(this.DebugTool_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buildCache;
        private System.Windows.Forms.Button getCoverButton;
        private System.Windows.Forms.TextBox artworkTime;
        private System.Windows.Forms.TextBox artistTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}