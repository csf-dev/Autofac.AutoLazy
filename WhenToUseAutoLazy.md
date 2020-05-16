# Why & when to use AutoLazy
There are two common dependency-injection problems which require lazily-resolved dependencies.

* Preventing invalid resolution
* Working-around circular dependencies

## Invalid resolution due to the application state
Sometimes, particularly when a complex resolution takes place, a component cannot be resolved due
to the current application state.  This can be particularly frustrating when the entry-point to
the application has to be constructed before the application state can be checked.

### Example: Web applications
A common example of this occurs within ASP.NET & dotnet core **MVC and Web API**.  A Controller
class may be decorated with `AuthorizeAttribute` in order to prevent usage by those who are
not logged-into the application.  However, the MVC/API frameworks will not test for the attribute
until *after the controller instance is constructed*.  In a dependency-injection environment, that
will include the construction of all of its dependencies.

Imagine that in the graph of dependencies there is a factory service, which requires information
about the currently-logged-in user.  This factory is used to construct one of the dependencies
for a controller.  Let's also imagine that this factory throws an exception when
*there is no currently-logged-in user*, because it cannot return a meaningful result.
Now, unauthenticated users browsing to that controller will see an internal server error
(HTTP 500) and not a forbidden response (HTTP 403), as the developer would have expected from the
`AuthorizeAttribute`.  This is because the crash (from the factory) occurs *before the authorization is tested*.

The easy way to solve this problem is to make the dependencies for your Controller classes **lazy**.
If they are not fully resolved until they are used, then the authorization-check will occur first,
as the developer expects.  The visitor is denied permission to execute the controller's action methods
and they see the responses which are expected.  The factory never raises an exception, because
the dependencies were never fully resolved.

## Circular dependencies
Before we go any further, *circular dependencies are usually a sign of poor design*.  Although
AutoLazy can help work around them, it should be considered just that: a workaround for an architectural
issue.  It is far better (if possible) to refactor your application and remove the circular
dependencies.

That said, circular dependency chains can be broken using AutoLazy. By marking one or more of the
interfaces in the dependency-chain as an auto-lazy interface, they will not form an endless resolution
loop.  This will be effective as long as *the actual execution paths of exercised logic are not circular*.
AutoLazy won't be able to help you if your dependencies truly form an endless loop of executed logic.

# Why handle this at the dependency injection level?
In both of the scenarios above, these problems could be solved *without using AutoLazy*.  The
developer could manually change the types of the dependency constructor parameters from `T` to
`Lazy<T>` and receive the same end-result.

However, for the scenarios described above, considering **[the dependency inversion principle]**,
whether a dependency needs to be lazily-resolved or not is *an implementation detail* and not a
part of the abstraction offered by the interface.

For example, the MVC/API Controller should not need to know that (perhaps behind multiple levels
of abstraction), there is a factory in its dependency graph which depends upon a
currently-logged-in-user.  The controller's code should not need to change in order to deal with
the way an unrelated object is constructed.

These scenarios describe *a dependency injection problem* and not a problem which lies in ths design
of *the consumer of the dependency*.  This also touches on **[open/closed principle]**, avoiding unwanted
changes to existing classes.  **AutoLazy** allows the consumer to continue to depend on simply
the interface, and solves the dependency injection problem in the dependency injection logic.

[the dependency inversion principle]: https://en.wikipedia.org/wiki/Dependency_inversion_principle
[open/closed principle]: https://en.wikipedia.org/wiki/Open%E2%80%93closed_principle
