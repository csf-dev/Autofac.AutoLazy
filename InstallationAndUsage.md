## Installing AutoLazy
To install AutoLazy into your project, first choose the appropriate package version corresponding to the version of Autofac you are using:

* `AutoLazy.Autofac4` (for Autofac versions 4.x)
* `AutoLazy.Autofac5` (for Autofac versions 5.x)

Reference this package *from the assembly where you create your container*.  Next, install the `Autofac.AutoLazyModule` from the package (an example follows).  Once this is done, you're ready to choose which services & consuming components should be auto-lazy.

```csharp
// builder is an Autofac ContainerBuilder
builder.RegisterModule<AutoLazyModule>();
```

## Using AutoLazy
AutoLazy provides two mechanisms by which a developer chooses which dependencies are resolved auto-lazily.  Each looks at the dependency *from a different angle*.  The first is based upon the interface which is depended-upon, the second is based upon the concrete class consuming dependencies.  Those two mechanism are:

* Select one or more *service interfaces*; whenever these services are resolved (no matter where they are consumed), they will be auto-lazy.
* Select *dependency-consuming component classes*, either by listing them or by providing a matching predicate; any dependencies consumed by a matching class (where the dependency is an interface type) will be auto-lazy.

A dependency will be resolved auto-lazily if *either* its service interface is chosen to be auto-lazy (the first mechanism) *or* if the consuming class has been chosen to have its dependencies provided lazily (the second mechanism).  Each usage of the functionality described in the following sections is **additive**.

### Selecting auto-lazy services
To select auto-lazy services, use the following syntax with an Autofac `ContainerBuilder`.  All of these overloads provide a parameter which selects whether **property injection** is handled or not-handled.  In all cases it defaults to true.

```csharp
// Choose a single service interface to always be auto-lazy.
builder.MakeAutoLazyInterface<IServiceInterface>();

// Choose a few services to always be auto-lazy,
// the false parameter indicates that property-injection
// will not be configured.
builder.MakeAutoLazyInterfaces(false,
                               typeof(IServiceInterface),
                               typeof(IDifferentServiceInterface),
                               typeof(IAnotherServiceInterface));

// All of the specified IEnumerable<Type> will always
// be auto-lazy.
builder.MakeAutoLazyInterfaces(enumerableOfServiceTypes);
```

### Selecting dependency-consumers to get auto-lazy dependencies
To select dependency-consuming components for which all dependencies should be lazy, use the following syntax with an Autofac `ContainerBuilder`.  All of these overloads provide a parameter which selects whether **property injection** is handled or not-handled.  In all cases it defaults to true.

```csharp
// All interfaces that MyConcreteConsumer depends-upon
// shall be auto-lazy.
builder.MakeConsumedInterfacesAutoLazy<MyConcreteConsumer>()

// Choose a few consumer types to get auto-lazy dependencies,
// the true parameter indicates that property-injection
// will be configured.
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
