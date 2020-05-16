using System;
using System.Collections.Generic;
using AutoLazy;
using AutoLazy.Autofac;

namespace Autofac
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
        /// <param name="handlePropertyInjection">If set to <c>true</c> then property-injection is handled,
        /// by setting any settable properties (of type <paramref name="serviceInterface"/>) of any resolved components
        /// to an auto-lazy instance of <paramref name="serviceInterface"/>.  If <c>false</c> then the property-injection
        /// process (if any) will not be altered.</param>
        public static void MakeAutoLazyInterface(this ContainerBuilder builder, Type serviceInterface, bool handlePropertyInjection = true)
        {
            InterfaceDetector.AssertIsInterface(serviceInterface);
            var moduleType = typeof(MakeAutoLazyByResolvedTypeModule<>).MakeGenericType(serviceInterface);
            var module = (Module)Activator.CreateInstance(moduleType, new object[] { handlePropertyInjection });
            builder.RegisterModule(module);
        }

        /// <summary>
        /// Configures the container to always resolve the specified interface as an AutoLazy component.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="handlePropertyInjection">If set to <c>true</c> then property-injection is handled,
        /// by setting any settable properties (of type <typeparamref name="T"/>) of any resolved components
        /// to an auto-lazy instance of <typeparamref name="T"/>.  If <c>false</c> then the property-injection
        /// process (if any) will not be altered.</param>
        /// <typeparam name="T">The service interface type.</typeparam>
        public static void MakeAutoLazyInterface<T>(this ContainerBuilder builder, bool handlePropertyInjection = true) where T : class
        {
            InterfaceDetector.AssertIsInterface<T>();
            builder.RegisterModule(new MakeAutoLazyByResolvedTypeModule<T>(handlePropertyInjection));
        }

        /// <summary>
        /// Configures the container to always resolve the specified interfaces as AutoLazy components.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="handlePropertyInjection">If set to <c>true</c> then property-injection is handled,
        /// by setting any settable properties (of any of the <paramref name="serviceInterfaces"/> types) of any resolved components
        /// to an auto-lazy instance of the applicable <paramref name="serviceInterfaces"/> tyoe.  If <c>false</c> then the property-injection
        /// process (if any) will not be altered.</param>
        /// <param name="serviceInterfaces">The service interface types.</param>
        public static void MakeAutoLazyInterfaces(this ContainerBuilder builder, bool handlePropertyInjection, params Type[] serviceInterfaces)
            => builder.MakeAutoLazyInterfaces(serviceInterfaces, handlePropertyInjection);

        /// <summary>
        /// Configures the container to always resolve the specified interfaces as AutoLazy components.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="serviceInterfaces">The service interface types.</param>
        /// <param name="handlePropertyInjection">If set to <c>true</c> then property-injection is handled,
        /// by setting any settable properties (of any of the <paramref name="serviceInterfaces"/> types) of any resolved components
        /// to an auto-lazy instance of the applicable <paramref name="serviceInterfaces"/> tyoe.  If <c>false</c> then the property-injection
        /// process (if any) will not be altered.</param>
        public static void MakeAutoLazyInterfaces(this ContainerBuilder builder, IEnumerable<Type> serviceInterfaces, bool handlePropertyInjection = true)
        {
            foreach (var serviceInterface in serviceInterfaces)
                builder.MakeAutoLazyInterface(serviceInterface, handlePropertyInjection);
        }
    }
}
