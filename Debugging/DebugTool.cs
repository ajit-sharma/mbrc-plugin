#region

using System;
using System.Threading;
using System.Windows.Forms;
using Ninject;

#endregion

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
            using (var kernel = new StandardKernel(new InjectionModule()))
            {
                var module = kernel.Get<LibraryModule>();
                module.SyncGetCovers("all", 0, 5);
            }
        }

        private void DebugTool_Load(object sender, EventArgs e)
        {
        }
    }
}