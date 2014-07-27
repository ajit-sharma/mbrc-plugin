using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace MusicBeePlugin.Debugging
{
    public partial class DebugTool : Form
    {
        public DebugTool()
        {
            InitializeComponent();
        }

        private void OnTestButtonClick(object sender, EventArgs e)
        {

        }

        private void getCoverButton_Click(object sender, EventArgs e)
        {
            var workerThread = new Thread(GetCovers)
            {
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            workerThread.Start();
        }

        private static void GetCovers()
        {
            Plugin.Instance.LibraryModule.SyncGetCovers("all", 0, 5);
        }

        private void DebugTool_Load(object sender, EventArgs e)
        {

        }
    }
}
