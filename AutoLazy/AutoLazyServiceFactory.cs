using System;
using Castle.DynamicProxy;

namespace AutoLazy
{
    /// <summary>
    /// A factory which creates auto-lazy services.  That is, convert a <see cref="Lazy{T}"/> into
    /// simply an instance of <c>T</c>, but where the underlying instance will not be resolved (via
    /// <see cref="Lazy{T}.Value"/>) until the service is first used.
    /// </summary>
    public class AutoLazyServiceFactory : IGetsAutoLazyServices
    {
        readonly IGetsStubs stubFactory;
        readonly IGetsAutoLazyInterceptors autoLazyInterceptorFactory;
        readonly IProxyGenerator proxyGenerator;

        /// <summary>
        /// Gets an auto-lazy service which will wrap the specified <see cref="Lazy{T}"/> instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This works by creating a DynamicProxy using <see cref="IProxyGenerator.CreateInterfaceProxyWithTargetInterface{TInterface}(TInterface, IInterceptor[])"/>.
        /// That proxy implements the interface <typeparamref name="T"/> and is initially satisfied by a stub
        /// implementation of that interface type.  The stub implementation is created via <see cref="IGetsStubs"/>.
        /// </para>
        /// <para>
        /// One interceptor is created and used with the proxy, and that interceptor is created via
        /// <see cref="IGetsAutoLazyInterceptors"/>.  That interceptor will redirect any invocation of any
        /// functionality to <see cref="Lazy{T}.Value"/> of the specified <paramref name="lazy"/>
        /// object.
        /// </para>
        /// </remarks>
        /// <returns>The auto-lazy service.</returns>
        /// <param name="lazy">A lazy instance, to wrap in an auto-lazy service.</param>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <exception cref="ArgumentNullException">If <paramref name="lazy"/> is <c>null</c>.</exception>
        /// <exception cref="AutoLazyException">If the service type <typeparamref name="T"/> is not valid to be an auto-lazy service.</exception>
        public T GetAutoLazyService<T>(Lazy<T> lazy) where T : class
        {
            if (lazy == null)
                throw new ArgumentNullException(nameof(lazy));
            InterfaceDetector.AssertIsInterface<T>();

            var lazyInterceptor = autoLazyInterceptorFactory.GetAutoLazyInterceptor(lazy);
            var stubImplementation = stubFactory.GetStub<T>();
            return proxyGenerator.CreateInterfaceProxyWithTargetInterface(stubImplementation, lazyInterceptor);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoLazyServiceFactory"/> class.
        /// </summary>
        /// <param name="stubFactory">A factory for creating stub object instances.</param>
        /// <param name="autoLazyInterceptorFactory">A factory for creating auto-lazy DynamicProxy interceptors.</param>
        /// <param name="proxyGenerator">A DynamicProxy generator.</param>
        public AutoLazyServiceFactory(IGetsStubs stubFactory,
                                      IGetsAutoLazyInterceptors autoLazyInterceptorFactory,
                                      IProxyGenerator proxyGenerator)
        {
            this.stubFactory = stubFactory ?? throw new ArgumentNullException(nameof(stubFactory));
            this.autoLazyInterceptorFactory = autoLazyInterceptorFactory ?? throw new ArgumentNullException(nameof(autoLazyInterceptorFactory));
            this.proxyGenerator = proxyGenerator ?? throw new ArgumentNullException(nameof(proxyGenerator));
        }
    }
}
