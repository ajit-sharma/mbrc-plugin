#region

using Ninject;
using NServiceKit.Configuration;

#endregion

namespace MusicBeePlugin
{
    internal class NinjectIocAdapter : IContainerAdapter
    {
        private readonly IKernel _kernel;

        public NinjectIocAdapter(IKernel kernel)
        {
            _kernel = kernel;
        }

        public T TryResolve<T>()
        {
            return _kernel.CanResolve<T>() ? _kernel.Get<T>() : default (T);
        }

        public T Resolve<T>()
        {
            return _kernel.Get<T>();
        }
    }
}