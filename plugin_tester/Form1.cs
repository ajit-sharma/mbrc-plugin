namespace plugin_tester
{
    using System;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        public Form1()
        {
            this.InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var entry = new MusicBeeRemoteCore.MusicBeeRemoteEntryPointImpl();
            var provider = new MyProvider(
                new PlayerApiAdapter(), 
                new PlaylistAdapter(), 
                new TrackAdapter(), 
                new LibraryAdapter(), 
                new NowPlayingApiAdapter());

            entry.init(true, provider);
        }
    }
}