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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.getmetatags = new System.Windows.Forms.Button();
            this.metafile = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Sync Lib";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.SyncLibary);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(93, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "CheckForChanges";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.CheckForChanges);
            // 
            // getmetatags
            // 
            this.getmetatags.Location = new System.Drawing.Point(13, 39);
            this.getmetatags.Name = "getmetatags";
            this.getmetatags.Size = new System.Drawing.Size(75, 23);
            this.getmetatags.TabIndex = 2;
            this.getmetatags.Text = "GetMetaTags";
            this.getmetatags.UseVisualStyleBackColor = true;
            this.getmetatags.Click += new System.EventHandler(this.getmetatags_Click);
            // 
            // metafile
            // 
            this.metafile.Location = new System.Drawing.Point(95, 41);
            this.metafile.Name = "metafile";
            this.metafile.Size = new System.Drawing.Size(401, 20);
            this.metafile.TabIndex = 3;
            // 
            // DebugTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 262);
            this.Controls.Add(this.metafile);
            this.Controls.Add(this.getmetatags);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "DebugTool";
            this.Text = "DebugTool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button getmetatags;
        private System.Windows.Forms.TextBox metafile;
    }
}