using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Core;
using AutoLazy;

namespace Autofac.AutoLazy
{
    /// <summary>
    /// A service which uses an Autofac component context (and optionally, parameters)
    /// to resolve auto-lazy service instances.
    /// </summary>
    public class AutoLazyResolver : IResolvesAutoLazyServices
    {
        /// <summary>
        /// Resolves an auto-lazy implementation of the service type
        /// <typeparamref name="T"/> (which must be an interface).
        /// </summary>
        /// <returns>The auto-lazy service implementations.</returns>
        /// <param name="ctx">An Autofac component context.</param>
        /// <param name="params">A collection of Autofac parameters.</param>
        /// <typeparam name="T">The desired service type (must be an interface).</typeparam>
        public T ResolveAutoLazyService<T>(IComponentContext ctx, IEnumerable<Parameter> @params = null) where T : class
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));

            var autoLazyFactory = ctx.Resolve<IGetsAutoLazyServices>();
            var lazyProvider = ctx.Resolve<LazyInstanceProvider<T>>(@params ?? Enumerable.Empty<Parameter>());
            return autoLazyFactory.GetAutoLazyService(lazyProvider.Instance);
        }
    }
}
