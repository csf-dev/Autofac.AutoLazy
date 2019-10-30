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
