using System;
using AutoLazy.Services;
using Autofac;

namespace AutoLazy
{
    public class AutoLazyTestsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<AutoLazyModule>();

            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .Where(t => t?.Namespace?.StartsWith(typeof(IServiceInterface).Namespace, StringComparison.InvariantCulture) == true)
                .AsSelf()
                .AsImplementedInterfaces();
        }
    }
}
