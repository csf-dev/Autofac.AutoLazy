# AutoLazy for Autofac
Provides a NuGet package for each of [**Autofac 4.x** and **Autofac 5.x**] which
enables selective automatic lazy-resolution of dependencies which are interfaces.
This means that **interfaces** (only) may be automatically proxied/wrapped, such
that the *real implementation* is not constructed/resolved until first usage.
This can be useful in [a number of circumstances].

[**Autofac 4.x** and **Autofac 5.x**]: https://autofac.org/
[a number of circumstances]: WhenToUseAutoLazy.md

## Example
Here is a sample component which makes use of a dependency.

```csharp
public interface IDependency
{
    void DoSomething();
}

public class DependencyImpl : IDependency
{
    public void DoSomething()
    {
        Console.WriteLine("Dependency executing");
    }

    public DependencyImpl()
    {
        Console.WriteLine("Constructing dependency");
    }
}

public class MyComponent
{
    readonly IDependency dependency;
    
    public void UseDependency()
    {
        Console.WriteLine("About to use dependency");
        dependency.DoSomething();
        Console.WriteLine("Finished using dependency");
    }
    
    public MyComponent(IDependency dependency)
    {
        Console.WriteLine("Constructing component");
        this.dependency = dependency
            ?? throw new ArgumentNullException(nameof(dependency));
    }
}
```

### Expected behaviour without AutoLazy
Without auto-lazy, presuming all of these types are registered, a developer would expect
the following messages logged in-order when resolving an instance of `MyComponent` from
an Autofac container and then executing its `UseDependency()` method.

```
Constructing dependency
Constructing component
About to use dependency
Dependency executing
Finished using dependency
```

### Expected behaviour with AutoLazy
When using AutoLazy and the following line of code (added to your Autofac registrations,
such as via [a Module]), the `DependencyImpl` won't be constructed until it is first-used.
This happens even though the constructor parameter of `MyComponent` is not `Lazy<IDependency>`.

To enable AutoLazy for the `IDependency` interface:

```csharp
builder.MakeAutoLazyInterface<IDependency>();
```

The output from the scenario described above (resolve `MyComponent` then execute
`UseDependency()` would instead be:

```
Constructing component
About to use dependency
Constructing dependency
Dependency executing
Finished using dependency
```

[a Module]: https://autofaccn.readthedocs.io/en/latest/configuration/modules.html

## How AutoLazy works
AutoLazy uses [Castle.DynamicProxy], a part of [Castle.Core], to create *a proxy object*
(essentially *a stand-in* object) which implements the same interface as the dependency
to be resolved.  That proxy wraps a `Lazy<T>` of the same interface, which has the 'real'
implementation of the service.

When any of the proxy object's functionality is used, the invocation is 'passed on' to
the `.Value` of the wrapped lazy object.  This causes the lazy object to be resolved as
normal (by Autofac) and the functionality to continue as it would have without AutoLazy.

[Castle.DynamicProxy]: http://www.castleproject.org/projects/dynamicproxy/
[Castle.Core]: https://github.com/castleproject/Core
