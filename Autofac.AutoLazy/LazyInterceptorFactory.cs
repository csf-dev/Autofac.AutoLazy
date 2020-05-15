using System;
using Castle.DynamicProxy;

namespace Autofac.AutoLazy
{
    /// <summary>
    /// A factory which creates <see cref="AutoLazyInterceptor{T}"/> instances.
    /// </summary>
    public class LazyInterceptorFactory : IGetsAutoLazyInterceptors
    {
        /// <summary>
        /// Gets a DynamicProxy <see cref="IInterceptor"/> for the specified <see cref="Lazy{T}"/>
        /// object.
        /// </summary>
        /// <returns>The auto-lazy interceptor.</returns>
        /// <param name="lazyImplementation">A lazy service implementation.</param>
        /// <typeparam name="T">The service type.</typeparam>
        public IInterceptor GetAutoLazyInterceptor<T>(Lazy<T> lazyImplementation) where T : class
            => new AutoLazyInterceptor<T>(lazyImplementation);
    }
}
