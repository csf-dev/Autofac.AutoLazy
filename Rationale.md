The problem that this attempts to solve, is for instances where eager resolution of a service type (from within a constructor) causes an exception, because (a long way down the dependency chain), logic is executed which is not "ready" to execute.

## Example
1. An MVC or WebAPI controller - in its constructor it depends upon an `IGetsFunds`
2. The `IGetsFunds` interface is created by a factory, which depends upon whether the user is a customer or not
3. The factory for getting the `IGetsFunds` depends upon a service which gets the currently-logged-in user and throws an exception if there is no user logged in
4. Someone tries to use the controller whilst not being logged-in

During the dependency resolution, the factory for the `IGetsFunds` interface will throw an exception, because there is no user logged-in.  In fact, marking that controller with `[Authorize]` attribute won't help, either, because the controller is constructed (and its dependencies resolved) before the authorize attribute is considered.

The solution to this is to alter the dependency upon `IGetsFunds` to `Lazy<IGetsFunds>`.  This way, it won't be resolved until it is being used, and thus the authorize attribute will take effect and prevent an unauthenticated visitor from using it.

## How Autofac can help
It would be nice, though, if we could *avoid needing to change the controller type* to require a lazy dependency, and rather give it an implementation of its dependency which does not suffer that weakness on resolution.

This need for a lazy implementation is a resolution-centric problem, so it should ideally have a resolution-centric solution, in order to keep everything simpler.

## Suggested fix
Use **Castle.DynamicProxy** to create a proxy of `IGetsFunds` which depends upon a `Lazy<IGetsFunds>` and forwards its calls onto the lazy instance, only initialising the lazy instance when it is first used, and not at construction.

The controller in the example would then still depend simply upon `IGetsFunds`, unaware that it is actually being given a proxy object, for which the full chain of dependency resolution has not yet occurred.