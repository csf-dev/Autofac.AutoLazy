using System;
using System.Reflection;

namespace AutoLazy
{
    /// <summary>
    /// A marker-class of sorts, which provides an instance of
    /// <see cref="Lazy{T}"/> per resolution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class exists so that instances of <see cref="Autofac.Core.ResolvedParameter"/>
    /// may 'detect' this class (by virtue of it being the <see cref="MemberInfo.DeclaringType"/>
    /// of the <see cref="ParameterInfo.Member"/> of the parameter used by the resolved parameter.
    /// </para>
    /// <para>
    /// That detection is required because otherwise the resolution process would get stuck in
    /// an endless loop of creating <see cref="Lazy{T}"/> instances which wrap one another.
    /// By having a class to detect, the resolved parameter's logic can be "lazily resolve all
    /// instances of T, but not if they were requested by a <see cref="LazyInstanceProvider{T}"/>".
    /// </para>
    /// </remarks>
    public sealed class LazyInstanceProvider<T> where T : class
    {
        /// <summary>
        /// Gets the lazy instance.
        /// </summary>
        /// <value>The lazy instance.</value>
        public Lazy<T> Instance { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyInstanceProvider{T}"/> class.
        /// </summary>
        /// <param name="lazy">A lazy object (which will point at the original implementation of <typeparamref name="T"/>).</param>
        public LazyInstanceProvider(Lazy<T> lazy)
        {
            InterfaceDetector.AssertIsInterface<T>();
            Instance = lazy ?? throw new ArgumentNullException(nameof(lazy));
        }
    }
}
