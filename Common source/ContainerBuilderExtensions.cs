using System;
using System.Collections.Generic;
using System.Linq;
using AutoLazy;
using AutoLazy.Autofac;

namespace Autofac
{
    /// <summary>
    /// <para>
    /// Extension methods for the <see cref="ContainerBuilder"/> type, related to AutoLazy.
    /// This provides two strategies for auto-lazy resolution.  Both strategies may be used together,
    /// a dependency is resolved auto-lazily if at least one mechanism has indicated it should be
    /// (a logical OR).
    /// </para>
    /// <para>
    /// One mechanism is by indicating the interface types
    /// which shall always be resolved lazily (no matter what consumes them).
    /// </para>
    /// <para>
    /// The other mechanism is by indicating
    /// the consuming types, which shall always have their interface-dependencies resolved lazily (no
    /// matter which interfaces those dependencies are).
    /// </para>
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Property injection is not configured by default; this corresponds to Autofac's
        /// default behaviour (where the developer needs to explicitly enable it).
        /// </summary>
        internal const bool DefaultPropertyInjection = false;

        #region AutoLazy based on the dependency interface

        /// <summary>
        /// Configures the container to always resolve the specified interface as an AutoLazy component.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="serviceInterface">The service interface type.</param>
        /// <param name="handlePropertyInjection">If set to <c>true</c> then property-injection is handled,
        /// by setting any settable properties (of type <paramref name="serviceInterface"/>) of any resolved components
        /// to an auto-lazy instance of <paramref name="serviceInterface"/>.  If <c>false</c> then the property-injection
        /// process (if any) will not be altered.</param>
        public static void MakeAutoLazyInterface(this ContainerBuilder builder,
                                                 Type serviceInterface,
                                                 bool handlePropertyInjection = DefaultPropertyInjection)
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
        public static void MakeAutoLazyInterface<T>(this ContainerBuilder builder,
                                                    bool handlePropertyInjection = DefaultPropertyInjection) where T : class
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
        public static void MakeAutoLazyInterfaces(this ContainerBuilder builder,
                                                  bool handlePropertyInjection,
                                                  params Type[] serviceInterfaces)
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
        public static void MakeAutoLazyInterfaces(this ContainerBuilder builder,
                                                  IEnumerable<Type> serviceInterfaces,
                                                  bool handlePropertyInjection = DefaultPropertyInjection)
        {
            if (serviceInterfaces == null) throw new ArgumentNullException(nameof(serviceInterfaces));
            foreach (var serviceInterface in serviceInterfaces)
                builder.MakeAutoLazyInterface(serviceInterface, handlePropertyInjection);
        }

        #endregion

        #region AutoLazy based on the type of the consumer

        /// <summary>
        /// Configures a dependency-consuming type, such that any dependencies
        /// it consumes (which are interfaces) will be resolved auto-lazily.
        /// Note that the consuming type is the concrete class requiring dependencies,
        /// and not its interface.
        /// </summary>
        /// <param name="builder">An Autofac container builder.</param>
        /// <param name="handlePropertyInjection">If set to <c>true</c> then property-injection is handled,
        /// by setting any settable properties on the resolved instance of <typeparamref name="T"/>, where the
        /// property-type is a resolvable interface, to an auto-lazy instance.
        /// If <c>false</c> then the property-injection process (if any) will not be altered.</param>
        /// <typeparam name="T">The type of the dependency-consuming component.</typeparam>
        public static void MakeConsumedInterfacesAutoLazy<T>(this ContainerBuilder builder,
                                                             bool handlePropertyInjection = DefaultPropertyInjection) where T : class
            => builder.MakeConsumedInterfacesAutoLazy(t => Equals(t, typeof(T)), handlePropertyInjection);

        /// <summary>
        /// Configures a collection of dependency-consuming types, such that any dependencies
        /// they consume (which are interfaces) will be resolved auto-lazily.
        /// Note that the consuming types are the concrete classes requiring dependencies,
        /// and not their interfaces.
        /// </summary>
        /// <param name="builder">An Autofac container builder.</param>
        /// <param name="handlePropertyInjection">If set to <c>true</c> then property-injection is handled,
        /// by setting any settable properties on the resolved instance of the <paramref name="consumerTypes"/>,
        /// where the property-type is a resolvable interface, to an auto-lazy instance.
        /// If <c>false</c> then the property-injection process (if any) will not be altered.</param>
        /// <param name="consumerTypes">The types of the dependency-consuming components.</param>
        public static void MakeConsumedInterfacesAutoLazy(this ContainerBuilder builder,
                                                          bool handlePropertyInjection,
                                                          params Type[] consumerTypes)
            => builder.MakeConsumedInterfacesAutoLazy(consumerTypes, handlePropertyInjection);

        /// <summary>
        /// Configures a collection of dependency-consuming types, such that any dependencies
        /// they consume (which are interfaces) will be resolved auto-lazily.
        /// Note that the consuming types are the concrete classes requiring dependencies,
        /// and not their interfaces.
        /// </summary>
        /// <param name="builder">An Autofac container builder.</param>
        /// <param name="consumerTypes">The types of the dependency-consuming components.</param>
        /// <param name="handlePropertyInjection">If set to <c>true</c> then property-injection is handled,
        /// by setting any settable properties on the resolved instance of the <paramref name="consumerTypes"/>,
        /// where the property-type is a resolvable interface, to an auto-lazy instance.
        /// If <c>false</c> then the property-injection process (if any) will not be altered.</param>
        public static void MakeConsumedInterfacesAutoLazy(this ContainerBuilder builder,
                                                          IEnumerable<Type> consumerTypes,
                                                          bool handlePropertyInjection = DefaultPropertyInjection)
            => builder.MakeConsumedInterfacesAutoLazy(t => consumerTypes.Contains(t), handlePropertyInjection);

        /// <summary>
        /// Configures dependency-consuming types matching a predicate, such that any dependencies
        /// they consume (which are interfaces) will be resolved auto-lazily.
        /// Note that the consuming types are the concrete classes requiring dependencies,
        /// and not their interfaces.
        /// </summary>
        /// <param name="builder">An Autofac container builder.</param>
        /// <param name="consumerTypePredicate">A predicate function which is used to determine
        /// which dependency-consumer types receive auto-lazy dependencies.</param>
        /// <param name="handlePropertyInjection">If set to <c>true</c> then property-injection is handled,
        /// by setting any settable properties on the resolved instance matching <paramref name="consumerTypePredicate"/>,
        /// where the property-type is a resolvable interface, to an auto-lazy instance.
        /// If <c>false</c> then the property-injection process (if any) will not be altered.</param>
        public static void MakeConsumedInterfacesAutoLazy(this ContainerBuilder builder,
                                                          Func<Type, bool> consumerTypePredicate,
                                                          bool handlePropertyInjection = DefaultPropertyInjection)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var module = new MakeAutoLazyByConsumerTypeModule(consumerTypePredicate, handlePropertyInjection);
            builder.RegisterModule(module);
        }

        #endregion
    }
}
