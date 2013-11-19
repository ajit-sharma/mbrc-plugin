using System;
using System.Windows.Forms;

namespace MusicBeePlugin.Debugging
{
    public partial class DebugTool : Form
    {
        public DebugTool()
        {
            InitializeComponent();
        }

        private void SyncLibary(object sender, EventArgs e)
        {
            Plugin.Instance.SyncGetFilenames("all");
        }

        private void CheckForChanges(object sender, EventArgs e)
        {
            //Plugin.Instance.SyncCheckForChanges(new DateTime(2013,11,1));
        }

        private void getmetatags_Click(object sender, EventArgs e)
        {
            Plugin.Instance.SyncGetMetaData(1, "all");
        }
    }
}
