public class AutoLazyServiceFactory : IGetsAutoLazyService
{
    readonly IGetsStubComponent stubComponentFactory;
    readonly IProxyGenerator proxyGenerator;
    readonly Func<Lazy<T>,AutoLazyInterceptor<T>> interceptorFactory;
    
    public T GetService<T>(Lazy<T> lazyService)
    {
        var interceptor = interceptorFactory(lazyService);
        var stubService = stubComponentFactory.GetStubComponent<T>();
        return proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(stubService, interceptor);
    }
    
    public AutoLazyServiceFactory(IGetsStubComponent stubComponentFactory,
                                  IProxyGenerator proxyGenerator,
                                  Func<Lazy<T>,AutoLazyInterceptor<T>> interceptorFactory)
    {
        this.stubComponentFactory = stubComponentFactory ?? throw new ArgumentNullException(nameof(stubComponentFactory));
        this.proxyGenerator = proxyGenerator ?? throw new ArgumentNullException(nameof(proxyGenerator));
        this.interceptorFactory = interceptorFactory ?? throw new ArgumentNullException(nameof(interceptorFactory));
    }
}