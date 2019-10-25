# Rationale for this project
There are two problems which present themselves from time to time when using dependency injection.

* Circular dependencies occasionally appear, which need to be 'broken' in order to resolve services sensibly.
* Logic which is required to resolve a service might not be valid to execute, due to the current application/environment state.

## Circular dependencies
Whilst in ideal circumstances, circular dependencies would never exist, sometimes they can occur. This is particularly true in complex applications where the circular dependency could involve a deeply nested tree of service/component dependencies.

A standard and easy way to 'break the circle' is to resolve one or more dependencies in the chain lazily. This way, the dependency is not resolved greedily. Unless actual usage of the service creates an infinite loop them the circle will remain broken via the lazy dependency. If usage does create an infinite loop, then there is nothing that can be done to help.

## Dealing with invalid resolution
Particularly when service/component resolution requires a factory with non-trivial logic, the resolution of services could raise an exception of the application is not in an expected state.

### An example: MVC controller
Consider the following example.

1. An MVC or WebAPI controller - in its constructor it depends upon an `IGetsFunds` service
2. The `IGetsFunds` component is created by a factory, which depends upon whether the current logged-in user is an existing customer or not
3. The factory for getting the `IGetsFunds` depends upon a service which gets the currently-logged-in user and throws an exception if there is no user logged in
4. Someone tries to use the controller whilst they are not logged-in

During the dependency resolution, the factory for the `IGetsFunds` interface will throw an exception, because there is no user logged-in. Of course, they shouldn't use this controller unless they are logged-in, and so the developer decorates the controller wry the `[Authorize]` attribute.

To the developer's surprise that doesn't work and they still see the exception. This is because the controller is constructed (and its dependencies resolved) before the authorize attribute is considered. The exception (during resolution) had already been thrown.

The solution to this is to alter the dependency upon `IGetsFunds` to `Lazy<IGetsFunds>`.  This way, it won't be resolved until it is being used, and thus the authorize attribute will take effect and prevent an unauthenticated visitor from using it.

## Why Autofac should handle this
In both of these scenarios above, the problem could be solved by simply changing the constructor dependency to `Lazy<T>` instead of `T`.

But.

Both of these problems are dependency resolution issues. The classes involved don't specifically need their services to be lazy, it's an implementation detail of the resolution process. So, it should be dealt with as part of dependency injection and not in the main application code.

It also helps us follow **Open/Closed Principle**, because it means there is no need to change a class just because its dependencies (perhaps many times removed) require lazy resolution.

## Development plan
Use **Castle.DynamicProxy** to create a proxy of the dependency service interface which itself depends upon a lazy instance of the actual service. The calls to the proxy's functionality are then forwarded onto the lazy instance.  Thus the 'real' instance is initialised lazily on first usage, but not when it is resolved.

By using dynamic proxy, the laziness does not need to be exposed. All consumers would still depend simply on the interface type. The laziness would be transparent.