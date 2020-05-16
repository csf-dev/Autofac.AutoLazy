using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Core;
using Autofac;
using System.Reflection;

namespace AutoLazy.Autofac
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
        /// <returns>The auto-lazy service implementation.</returns>
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

        /// <summary>
        /// Resolves an auto-lazy implementation of the service type
        /// <paramref name="serviceType"/> (which must be an interface).
        /// </summary>
        /// <returns>The auto-lazy service implementation.</returns>
        /// <param name="ctx">An Autofac component context.</param>
        /// <param name="serviceType">The desired service type (must be an interface).</param>
        /// <param name="params">A collection of Autofac parameters.</param>
        public object ResolveAutoLazyService(IComponentContext ctx, Type serviceType, IEnumerable<Parameter> @params = null)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));

            var method = GetType().GetMethods()
                .Where(x => x.Name == nameof(ResolveAutoLazyService)
                            && x.IsGenericMethodDefinition)
                .First();

            return method.MakeGenericMethod(serviceType).Invoke(this, new object[] { ctx, @params });
        }
    }
}
