using System;
using Castle.DynamicProxy;

namespace AutoLazy
{
    /// <summary>
    /// A DynamicProxy interceptor which changes the target for any invocation, provided the
    /// invocation implements <see cref="IChangeProxyTarget"/>.
    /// </summary>
    /// <typeparam name="T">The type of service which this implementation will resolve.</typeparam>
    public class AutoLazyInterceptor<T> : IInterceptor where T : class
    {
        readonly Lazy<T> lazyImplementation;

        /// <summary>
        /// Intercept the specified invocation.
        /// </summary>
        /// <param name="invocation">Invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            if (!(invocation is IChangeProxyTarget targetChanger)) return;
            targetChanger.ChangeInvocationTarget(lazyImplementation.Value);
            invocation.Proceed();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoLazyInterceptor{T}"/> class.
        /// </summary>
        /// <param name="lazyImplementation">A lazy implementation of the service type <typeparamref name="T"/>.</param>
        public AutoLazyInterceptor(Lazy<T> lazyImplementation)
        {
            this.lazyImplementation = lazyImplementation ?? throw new ArgumentNullException(nameof(lazyImplementation));
        }
    }
}
