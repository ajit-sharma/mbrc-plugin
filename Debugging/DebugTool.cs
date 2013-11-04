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
            Plugin.Instance.SyncLibrary();
        }

        private void CheckForChanges(object sender, EventArgs e)
        {
            Plugin.Instance.CheckForLibaryChanges();
        }
    }
}
