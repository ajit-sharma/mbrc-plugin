namespace MusicBeeRemoteTester
{
    using System;
    using System.Diagnostics;
    using System.IO.Pipes;

    class IpcServer
    {
        private NamedPipeServerStream namedPipe;

        private bool isStopping;

        public const string MusicBeeRemote = "musicbee-remote-pipe";
        public void start()
        {
            try
            {
                this.namedPipe = new NamedPipeServerStream(MusicBeeRemote, PipeDirection.InOut);
                this.namedPipe.BeginWaitForConnection(Callback, null);
            }
            catch (Exception ex)
            {
                
                Debug.WriteLine(ex);
                    
            }
            
        }

        private void Callback(IAsyncResult ar)
        {
           
        }
    }
}