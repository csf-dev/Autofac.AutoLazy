public interface IGetsStubComponent
{
    T GetStubComponent<T>();
}

public class StubComponentFactory : IGetsStubComponent
{
    readonly IProxyGenerator proxyGenerator;
    readonly Func<StubInterceptor> stubInterceptorFactory;
    
    public T GetStubComponent<T>()
    {
        var interceptor = stubInterceptorFactory();
        return proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(interceptor);
    }
    
    public StubComponentFactory(IProxyGenerator proxyGenerator,
                                Func<StubInterceptor> stubInterceptorFactory)
    {
        this.proxyGenerator = proxyGenerator ?? throw new ArgumentNullException(nameof(proxyGenerator));
        this.stubInterceptorFactory = stubInterceptorFactory ?? throw new ArgumentNullException(nameof(stubInterceptorFactory));
    }
}

public class StubInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        // For all functionality executed by the invocation, if it has a void return type
        // then do nothing at all.  If it has a non-void return type then return the default
        // instance of that type.
        // In all circumstances, ignore all parameters.
    }
}