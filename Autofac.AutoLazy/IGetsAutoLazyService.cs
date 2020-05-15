using System;
namespace Autofac.AutoLazy
{
    /// <summary>
    /// <para>
    /// An object which can convert a lazy object which exposes a service
    /// into an auto-lazy service.
    /// </para>
    /// <para>
    /// An auto-lazy service exposes the same type/interface as
    /// the original, wrapping the provided <see cref="Lazy{T}"/>, but does not read the
    /// <c>.Value</c> property until first usage.
    /// </para>
    /// </summary>
    public interface IGetsAutoLazyService
    {
        /// <summary>
        /// Gets an auto-lazy service which will wrap the specified <see cref="Lazy{T}"/> instance.
        /// </summary>
        /// <returns>The auto-lazy service.</returns>
        /// <param name="lazy">A lazy instance, to wrap in an auto-lazy service.</param>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <exception cref="ArgumentNullException">If <paramref name="lazy"/> is <c>null</c>.</exception>
        /// <exception cref="AutoLazyException">If the service type <typeparamref name="T"/> is not valid to be an auto-lazy service.</exception>
        T GetAutoLazyService<T>(Lazy<T> lazy) where T : class;
    }
}
