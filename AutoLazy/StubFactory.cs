﻿using System;
using Castle.DynamicProxy;

namespace AutoLazy
{
    /// <summary>
    /// A factory for stub object which uses Castle DynamicProxy.
    /// </summary>
    public class StubFactory : IGetsStubs
    {
        readonly IProxyGenerator proxyGenerator;
        readonly Func<IInterceptor> interceptorFactory;

        /// <summary>
        /// Gets a stub instance of the specified type.
        /// </summary>
        /// <returns>A stub object.</returns>
        /// <typeparam name="T">The type of the stub to create.</typeparam>
        public T GetStub<T>() where T : class
        {
            InterfaceDetector.AssertIsInterface<T>();
            var interceptor = interceptorFactory();
            return proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(interceptor);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StubFactory"/> class.
        /// </summary>
        /// <param name="proxyGenerator">A DynamicProxy proxy generator.</param>
        /// <param name="interceptorFactory">A function which creates a DynamicProxy interceptor.</param>
        public StubFactory(IProxyGenerator proxyGenerator, Func<IInterceptor> interceptorFactory)
        {
            this.proxyGenerator = proxyGenerator ?? throw new ArgumentNullException(nameof(proxyGenerator));
            this.interceptorFactory = interceptorFactory ?? throw new ArgumentNullException(nameof(interceptorFactory));
        }
    }
}
