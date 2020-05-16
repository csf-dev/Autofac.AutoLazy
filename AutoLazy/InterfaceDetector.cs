using System;
using System.Reflection;

namespace AutoLazy
{
    /// <summary>
    /// Provides an assertion that a specified type is an interface.
    /// </summary>
    public static class InterfaceDetector
    {
        /// <summary>
        /// Throws <see cref="AutoLazyException"/> if <typeparamref name="T"/> is not an interface.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        public static void AssertIsInterface<T>()
            => AssertIsInterface(typeof(T));

        /// <summary>
        /// Throws <see cref="AutoLazyException"/> if <typeparamref name="T"/> is not an interface.
        /// </summary>
        /// <param name="type">The type to test.</param>
        public static void AssertIsInterface(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!type.GetTypeInfo().IsInterface)
                throw new AutoLazyException($"Auto-lazy services must be interfaces (even abstract classes will not suffice).\n" +
                                            $"The type {type.Name} is not an interface.");
        }

    }
}
