public class StubInterceptor : IInterceptor
{
    /// <summary>
    /// Ignore all parameters from the invocation.  If the method returns non-void then set the
    /// return value to the default value, equivalent to <c>default(T)</c>.  Do nothing more.
    /// </summary>
    public void Intercept(IInvocation invocation)
    {
        var returnType = invocation.Method.ReturnType;
        if(returnType == typeof(void)) return;
        
        var isValueType = returnType.GetTypeInfo().IsValueType;
        invocation.ReturnValue = isValueType? Activator.CreateInstance(returnType) : null;
    }
}