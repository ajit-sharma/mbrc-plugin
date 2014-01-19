using System;
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

        private void RunCacheBuilding()
        {
            Plugin.Instance.SyncModule.BuildCache();
            Plugin.Instance.SyncModule.BuildCoverCache();
            Plugin.Instance.SyncModule.BuildArtistCoverCache();
        }

        private void OnTestButtonClick(object sender, EventArgs e)
        {
            Thread workerThread = new Thread(RunCacheBuilding);
            workerThread.IsBackground = true;
            workerThread.Priority = ThreadPriority.AboveNormal;
            workerThread.Start();

//            Plugin.Instance.SyncGetFilenames("all");
//            DateTime start = DateTime.Now;
//            textBox1.Text = @"Sync in progress...";
//            progressBar1.Maximum = 16262;
//            {
//                Plugin.Instance.SyncGetMetaData(i, "all");
//            }
//            textBox1.Text = @"Sync in complete...";
//            textBox3.Text = (DateTime.Now - start).ToString();
        }

        private void OnTestButtonTwoClick(object sender, EventArgs e)
        {            
        }
    }
}
