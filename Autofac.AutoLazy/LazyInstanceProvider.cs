using System;
namespace Autofac.AutoLazy
{
    public sealed class LazyInstanceProvider<T> where T : class
    {
        public Lazy<T> Instance { get; }

        public LazyInstanceProvider(Lazy<T> lazy)
        {
            InterfaceDetector.AssertIsInterface<T>();
            Instance = lazy ?? throw new ArgumentNullException(nameof(lazy));
        }
    }
}
