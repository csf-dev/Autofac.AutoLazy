using System;
using System.Reflection;

namespace Autofac.AutoLazy
{
    /// <summary>
    /// Extension methods for the <see cref="ContainerBuilder"/> type, related to AutoLazy.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Configures the container to always resolve the specified interface as an AutoLazy component.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="serviceInterface">The service interface type.</param>
        public static void MakeAutoLazyInterface(this ContainerBuilder builder, Type serviceInterface)
        {
            InterfaceDetector.AssertIsInterface(serviceInterface);
            var moduleType = typeof(MakeAutoLazyByResolvedTypeModule<>).MakeGenericType(serviceInterface);
            var module = (Module)Activator.CreateInstance(moduleType);
            builder.RegisterModule(module);
        }

        /// <summary>
        /// Configures the container to always resolve the specified interface as an AutoLazy component.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <typeparam name="T">The service interface type.</typeparam>
        public static void MakeAutoLazyInterface<T>(this ContainerBuilder builder) where T : class
        {
            InterfaceDetector.AssertIsInterface<T>();
            builder.RegisterModule<MakeAutoLazyByResolvedTypeModule<T>>();
        }
    }
}
