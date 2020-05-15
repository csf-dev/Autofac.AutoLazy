﻿using System;
using Castle.DynamicProxy;
using Autofac.Core;

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
                .RegisterAssemblyTypes(ThisAssembly)
                .AsSelf()
                .AsImplementedInterfaces()
                .PreserveExistingDefaults();

            builder
                .Register(ctx => ctx.Resolve<StubFactory>(GetStubInterceptorParameter()))
                .As<IGetsStubs>();

            if (registerProxyGenerator)
            {
                builder.RegisterType<ProxyGenerator>()
                    .AsSelf()
                    .As<IProxyGenerator>()
                    .SingleInstance();
            }
        }

        ResolvedParameter GetStubInterceptorParameter()
        {
            return new ResolvedParameter((param, ctx) => param.ParameterType == typeof(Func<IInterceptor>),
                                         (param, ctx) => new Func<IInterceptor>(ctx.Resolve<Func<StubInterceptor>>()));
        }

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
