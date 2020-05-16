using System;
using Castle.DynamicProxy;
using Autofac.Core;
using System.Reflection;
using AutoLazy;

namespace Autofac.AutoLazy
{
    /// <summary>
    /// Enacapsulates all of the dependency injection registration which needs to occur in order
    /// to use AutoLazy.
    /// </summary>
    public class AutoLazyModule : Module
    {
        readonly bool registerProxyGenerator;

        /// <summary>
        /// Adds the registrations required to use AutoLazy.
        /// </summary>
        /// <param name="builder">The Autofac container builder.</param>
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterAssemblyTypes(ThisAssembly, typeof(AutoLazyServiceFactory).GetTypeInfo().Assembly)
                .Where(x => !IsOptionalModule(x))
                .AsSelf()
                .AsImplementedInterfaces()
                .PreserveExistingDefaults();

            builder
                .Register(ctx => ctx.Resolve<StubFactory>(GetStubInterceptorParameter()))
                .As<IGetsStubs>();

            builder.RegisterGeneric(typeof(LazyInstanceProvider<>));

            if (registerProxyGenerator)
            {
                builder.RegisterType<ProxyGenerator>()
                    .AsSelf()
                    .As<IProxyGenerator>()
                    .SingleInstance();
            }
        }

        bool IsOptionalModule(Type type)
        {
            if (type == null) return false;
            var typeInfo = type.GetTypeInfo();

            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(MakeAutoLazyByResolvedTypeModule<>))
                return true;

            return false;
        }

        ResolvedParameter GetStubInterceptorParameter()
        {
            return new ResolvedParameter((param, ctx) => param.ParameterType == typeof(Func<IInterceptor>),
                                         (param, ctx) => new Func<IInterceptor>(ctx.Resolve<Func<StubInterceptor>>()));
        }

        /// <summary>
        /// <para>
        /// Initializes a new instance of the <see cref="AutoLazyModule"/> class, in a manner
        /// which will register the proxy generator automatically.
        /// </para>
        /// <para>
        /// This constructor is required in order for consumers to use <c>ContainerBuilder.RegisterModule&lt;T&gt;()</c>.
        /// </para>
        /// </summary>
        public AutoLazyModule() : this(true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoLazyModule"/> class.
        /// </summary>
        /// <param name="registerProxyGenerator">If set to <c>true</c> then this module will register <see cref="ProxyGenerator"/> as
        /// <see cref="IProxyGenerator"/> for you (single-instance).  Set this to false if you do not want this module to handle the registration of
        /// the proxy generator.  For example, if you use DynamicProxy in your consuming logic and already register it yourself.</param>
        public AutoLazyModule(bool registerProxyGenerator = true)
        {
            this.registerProxyGenerator = registerProxyGenerator;
        }
    }
}
