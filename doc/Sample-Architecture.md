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

## Interface for a service to resolve lazy components
This will be the interface for the service which resolves lazy components.  The implementation would have a dependency upon a castle dynamic proxy `ProxyGenerator` instance. Its method signature will accept the normal Autofac resolution parameters (a component context and autofac parameters).

```csharp
public interface IGetsLazyServices
{
  T GetLazyService<T>(IComponentContext ctx, IEnumerable<Parameter> afParams);
}
```

Its rough behaviour should be:

1. Create a small `Func<T>` which will resolve the *real component instance* from the component context (passing the autofac parameters also).
    * It may be needed to pass an additional autofac parameter to prevent infinite resolution loops
2. Use this function/lambda to create a `Lazy<T>`, with the lambda as its constructor parameter
3. Create an interceptor (as described above), using that `Lazy<T>` as a dependency
4. Create an **interface without target** proxy, which will serve as a stub implementation (is never actually used)
    * It will need one interceptor, which should return default for the type (for methods with return values). It should ignore all parameters.
5. Create a dynamic **interface with target interface** proxy, using:
    * The proxy generator from the constructor dependency
    * The interceptor created in the step 3
    * The stub implementation created in the previous step
6. Return that proxy from step 5, cast to the component interface

### Registering the proxy generator
The proxy generator itself must be registered as **a singleton**; apparently there should only be one per app, or it performs badly.

## Unlikely to use Autofac's built in proxy support
Autofac has a built-in support for Castle.DynamicProxy, but actually I'm not sure I want to use it for this.  I think that it's not completely relevant for this scenario.  Still, it's described in the following two articles.

* https://github.com/autofac/Autofac.Extras.DynamicProxy
* https://autofac.readthedocs.io/en/latest/advanced/interceptors.html