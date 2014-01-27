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

        private void RunCacheBuilding()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Plugin.Instance.SyncModule.BuildCache();
            sw.Stop();
            Debug.WriteLine("Basic Cache build in {0}", sw.Elapsed);
            sw.Reset();
            sw.Start();
            Plugin.Instance.SyncModule.BuildCoverCache();
            sw.Stop();
            Debug.WriteLine("Artwork Cache build in {0}", sw.Elapsed);
            sw.Reset();
            sw.Start();
            Plugin.Instance.SyncModule.BuildArtistCoverCache();
            sw.Stop();
            Debug.WriteLine("Artist Artwork Cache build in {0}", sw.Elapsed);
            sw.Reset();
            sw.Start();
        }

        private void OnTestButtonClick(object sender, EventArgs e)
        {
            Thread workerThread = new Thread(RunCacheBuilding);
            workerThread.IsBackground = true;
            workerThread.Priority = ThreadPriority.AboveNormal;
            workerThread.Start();
        }

        private void getCoverButton_Click(object sender, EventArgs e)
        {
            Thread workerThread = new Thread(GetCovers);
            workerThread.IsBackground = true;
            workerThread.Priority = ThreadPriority.AboveNormal;
            workerThread.Start();
        }

        private void GetCovers()
        {
            Plugin.Instance.SyncModule.SyncGetCover("","");
            int count = 0;
            int limit = 5;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (count < 1000)
            {
                Plugin.Instance.SyncModule.SyncGetCovers(count,"all", limit);
                count += limit;
            }
            sw.Stop();
            Debug.WriteLine("Time Elapsed {0}", sw.Elapsed);
        }

        private void DebugTool_Load(object sender, EventArgs e)
        {

        }
    }
}
