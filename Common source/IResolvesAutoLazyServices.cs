using System.Collections.Generic;
using Autofac.Core;
using Autofac;

namespace AutoLazy.Autofac
{
    /// <summary>
    /// An object which resolves instances of arbitrary service types (provided the type is an interface),
    /// returning auto-lazy implementations.
    /// </summary>
    public interface IResolvesAutoLazyServices
    {
        /// <summary>
        /// Resolves an auto-lazy implementation of the service type
        /// <typeparamref name="T"/> (which must be an interface).
        /// </summary>
        /// <returns>The auto-lazy service implementations.</returns>
        /// <param name="ctx">An Autofac component context.</param>
        /// <param name="params">A collection of Autofac parameters.</param>
        /// <typeparam name="T">The desired service type (must be an interface).</typeparam>
        T ResolveAutoLazyService<T>(IComponentContext ctx, IEnumerable<Parameter> @params = null) where T : class;
    }
}
