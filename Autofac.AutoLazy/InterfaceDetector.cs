using System;
using System.Reflection;

namespace Autofac.AutoLazy
{
    internal static class InterfaceDetector
    {
        internal static void AssertIsInterface<T>()
            => AssertIsInterface(typeof(T));

        internal static void AssertIsInterface(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!type.GetTypeInfo().IsInterface)
                throw new AutoLazyException($"Auto-lazy services must be interfaces (even abstract classes will not suffice).\n" +
                                            $"The type {type.Name} is not an interface.");
        }

    }
}
