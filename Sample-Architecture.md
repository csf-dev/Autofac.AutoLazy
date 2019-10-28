# A sample/untested architecture
## An interceptor
The **castle DynamicProxy interceptor** would be generic for the service (interface) type. It would take (in its constructor) a lazy instance of the service as a dependency.

```csharp
public class AutoLazyInterceptor<T> : IInterceptor
{
  readonly Lazy<T> lazyService;

  // Interceptor implementation omitted

  public AutoLazyInterceptor(Lazy<T> lazyService)
  {
    this.lazyService = lazyService;
  }
}
```

When intercepting, the interceptor class will get the value from the lazy service and [substitute it as the target for the invocation](https://kozmic.net/2009/04/27/castle-dynamic-proxy-tutorial-part-x-interface-proxies-with-target/). This is possible by casting the invocation parameter itself into [the `IChangeProxyTarget` interface](https://github.com/castleproject/Core/blob/master/src/Castle.Core/DynamicProxy/IChangeProxyTarget.cs). This is [demonstrated here](https://stackoverflow.com/a/17967944/6221779).

This way, the lazy instance is resolved with usage, not at resolution time.

## An auto-lazy factory service
```csharp
public interface IGetsLazyServices
{
  T GetLazyService<T>(IComponentContext ctx, IEnumerable<Parameter> afParams);
}
```

The implementation would be a class which has a dependency upon a castle dynamic proxy `ProxyGenerator`. That proxy generator itself must be registered as **a singleton**; apparently there should only be one per app, or it performs badly.

It would create a small lambda which is a `Func<T>`, and is uses as the parameter to a `Lazy<T>`. That lambda will have a body which uses the component context and parameters in order to resolve the actual implementation of the service type.

That lazy object will be passed as the constructor parameter to the auto-lazy interceptor class (above).

Finally it will use the proxy generator to create a proxy of the target interface, using the interceptor created above. This will be the return value.

## Unlikely to use Autofac's built in proxy support
Autofac has a built-in support for Castle.DynamicProxy, but actually I'm not sure I want to use it for this.  I think that it's not completely relevant for this scenario.  Still, it's described in the following two articles.

* https://github.com/autofac/Autofac.Extras.DynamicProxy
* https://autofac.readthedocs.io/en/latest/advanced/interceptors.html