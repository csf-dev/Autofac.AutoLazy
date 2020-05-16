using System;
using Autofac;

namespace AutoLazy
{
    public class CachingContainerProvider
    {
        static IContainer cachedContainer;
        static object syncRoot = new object();

        public IContainer GetContainer()
        {
            lock(syncRoot)
            {
                if (cachedContainer == null)
                    cachedContainer = CreateContainer();
            }

            return cachedContainer;
        }

        static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<AutoLazyTestsModule>();
            return builder.Build();
        }
    }
}
