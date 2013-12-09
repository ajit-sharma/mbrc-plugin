using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MusicBeePlugin.Debugging
{
    public partial class DebugTool : Form
    {
        public DebugTool()
        {
            InitializeComponent();
        }

        private void getmetatags_Click(object sender, EventArgs e)
        {
            Plugin.Instance.SyncGetFilenames("all");
            DateTime start = DateTime.Now;
            textBox1.Text = @"Sync in progress...";
            progressBar1.Maximum = 16000;
            for (int i = 0; i <= 16000; i++)
            {
                Plugin.Instance.SyncGetMetaData(i, "all");
            }
            textBox1.Text = @"Sync in complete...";
            textBox3.Text = (DateTime.Now - start).ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        
    }
}
