using MusicBeeRemoteCore.Core.ApiAdapters;
using StructureMap;

namespace MusicBeeRemoteCore.Core.Windows
{
    class WindowManager : IWindowManager
    {
        private readonly IInvokeHandler _invokeHandler;
        private readonly IContainer _container;
        private InfoWindow _window;

        public WindowManager(IInvokeHandler invokeHandler, IContainer container)
        {
            _invokeHandler = invokeHandler;
            _container = container;
        }

        public void DisplayInfoWindow()
        {
            _invokeHandler.Invoke(DisplayWindow);
        }

        private void DisplayWindow()
        {
            if (_window == null || !_window.Visible)
            {
                _window = _container.GetInstance<InfoWindow>();
                //_window.SetOnDebugSelectionListener(this); todo replace with some kind of event
            }

            _window.Show();
        }
    }
}