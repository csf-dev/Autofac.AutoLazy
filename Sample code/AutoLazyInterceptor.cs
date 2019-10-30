public class AutoLazyInterceptor<T> : IInterceptor
{
    readonly Lazy<T> lazyService;

    /// <summary>
    /// Upon any intercepted invocation, get the value from the lazy implementation
    /// and replace the proxy target with that instance.
    /// </summary>
    public void Intercept(IInvocation invocation)
    {
        var targetChanger = invocation as IChangeProxyTarget;
        if (targetChanger == null) return;
        
        targetChanger.ChangeInvocationTarget(lazyService.Value);
    }

  public AutoLazyInterceptor(Lazy<T> lazyService)
  {
    this.lazyService = lazyService ?? throw new ArgumentNullException(nameof(lazyService));;
  }
}