## Installing AutoLazy
To install AutoLazy into your solution, first choose the appropriate NuGet package, corresponding to the version of Autofac you are using.  Both AutoLazy packages depend on **Castle.Core** v4.0.0 or higher.

* [`AutoLazy.Autofac4`] *for Autofac versions 4.2.0 up to (but not including) 5.0.0*
* [`AutoLazy.Autofac5`] *for Autofac versions 5.0.0 and higher*

Install the AutoLazy package *to the assembly where you create the Autofac container*; business logic projects which do not depend directly upon Autofac *do not need to depend on AutoLazy either*.  Next, register the `Autofac.AutoLazyModule` into the `ContainerBuilder`:

```csharp
builder.RegisterModule<AutoLazyModule>();
```

With this done, you're ready to choose which services & consuming components should be auto-lazy.

[`AutoLazy.Autofac4`]: https://www.nuget.org/packages/AutoLazy.Autofac4/
[`AutoLazy.Autofac5`]: https://www.nuget.org/packages/AutoLazy.Autofac5/

## Using AutoLazy
AutoLazy provides two mechanisms by which a developer chooses which dependencies are resolved auto-lazily.  These mechanisms allow the developer to select auto-lazy resolution *from 'either side' of the dependency*.  Remember that AutoLazy *can only lazily-resolve dependencies which are consumed as interfaces*, though.

*   The first mechanism looks at the interfaces which are *depended-upon*.  The developer selects one or more *service interfaces*.  Whenever these services are resolved (no matter where they are consumed), they will be auto-lazy.
*   The second mechanism looks at the concrete classes *consuming dependencies*.  The developer selects *dependency-consuming component classes*, either by listing them or by providing a matching predicate.  Any dependencies (which are interfaces) consumed by a matching class will be auto-lazy.

Usages of these mechanisms is **additive**.  A dependency will be resolved auto-lazily if *either* its interface is made auto-lazy (via the first mechanism) *or* if its consumer is made to receive auto-lazy dependencies (the second mechanism).

### Selecting auto-lazy services
To select auto-lazy services, use the following syntax with an Autofac `ContainerBuilder`.  All but one of these overloads provide an optional parameter which selects whether **property injection** is handled or not-handled.  Where this parameter is optional, its default value is **false**.

```csharp
// Choose a single service interface to always be auto-lazy.
builder.MakeAutoLazyInterface<IServiceInterface>();

// Choose a few services to always be auto-lazy.
// The false parameter indicates that property-injection
// will not be configured; this parameter is mandatory
// for this overload.
builder.MakeAutoLazyInterfaces(false,
                               typeof(IServiceInterface),
                               typeof(IDifferentServiceInterface),
                               typeof(IAnotherServiceInterface));

// All of the specified IEnumerable<Type> will always
// be auto-lazy.
builder.MakeAutoLazyInterfaces(enumerableOfServiceTypes);
```

### Selecting dependency-consumers to get auto-lazy dependencies
To select dependency-consuming components for which all dependencies should be lazy, use the following syntax with an Autofac `ContainerBuilder`.  All but one of these overloads provide an optional parameter which selects whether **property injection** is handled or not-handled.  Where this parameter is optional, its default value is **false**.

```csharp
// All interfaces that MyConcreteConsumer depends-upon
// shall be auto-lazy.
builder.MakeConsumedInterfacesAutoLazy<MyConcreteConsumer>()

// Choose a few consumer types to get auto-lazy dependencies.
// The true parameter indicates that property-injection
// will be configured; this parameter is mandatory
// for this overload.
builder.MakeConsumedInterfacesAutoLazy(true,
                                       typeof(AServiceClass),
                                       typeof(ADifferentServiceClass),
                                       typeof(AnotherServiceClass));

// All of the specified IEnumerable<Type> will receive their
// dependencies auto-lazily.
builder.MakeConsumedInterfacesAutoLazy(enumerableOfDependencyConsumerTypes);

// Any classes which match the predicate (all controller types)
// will receive their dependencies auto-lazily.
builder.MakeConsumedInterfacesAutoLazy(t => typeof(Controller).IsAssignableFrom(t));
```
