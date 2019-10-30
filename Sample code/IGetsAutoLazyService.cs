public interface IGetsAutoLazyService
{
    T GetService<T>(Lazy<T> lazyService);
}