# Planned usage
When using this as a developer writing/modifying Autofac registrations in my project, I'd like to have two ways to do it:

* As an extension method available to registration. When used, it means that the service resolved from that registration will always be a lazy implementation.
* As a resolution hook within a module. This way, we can inspect the class performing the resolution and decide that its dependencies are to be lazy, no matter what they are.

In order words, I could mark the registration as lazy, or mark the resolution as lazy.