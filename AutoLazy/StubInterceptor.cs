using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace AutoLazy
{
    /// <summary>
    /// <para>
    /// A DynamicProxy interceptor which provides stub-only behaviour.
    /// </para>
    /// <para>
    /// Any invocation of any functionality will always ignore any parameters present, and
    /// if any return value is present, it will always be the default of the data-type.
    /// </para>
    /// </summary>
    public class StubInterceptor : IInterceptor
    {
        /// <summary>
        /// Intercept the specified invocation.
        /// </summary>
        /// <param name="invocation">Invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            var returnType = invocation.Method.ReturnType;
            if (returnType == typeof(void)) return;

            invocation.ReturnValue = GetDefaultOfType(returnType);
        }

        object GetDefaultOfType(Type type)
        {
            var isValueType = type.GetTypeInfo().IsValueType;
            return isValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
