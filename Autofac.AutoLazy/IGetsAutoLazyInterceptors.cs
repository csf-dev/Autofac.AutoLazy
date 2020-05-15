using System;
using Castle.DynamicProxy;

namespace Autofac.AutoLazy
{
    /// <summary>
    /// An object which can create DynamicProxy <see cref="IInterceptor"/> instances
    /// for a specified type, using a <see cref="Lazy{T}"/> object.
    /// </summary>
    public interface IGetsAutoLazyInterceptors
    {
        /// <summary>
        /// Gets a DynamicProxy <see cref="IInterceptor"/> for the specified <see cref="Lazy{T}"/>
        /// object.
        /// </summary>
        /// <returns>The auto-lazy interceptor.</returns>
        /// <param name="lazyImplementation">A lazy service implementation.</param>
        /// <typeparam name="T">The service type.</typeparam>
        IInterceptor GetAutoLazyInterceptor<T>(Lazy<T> lazyImplementation) where T : class;
    }
}
