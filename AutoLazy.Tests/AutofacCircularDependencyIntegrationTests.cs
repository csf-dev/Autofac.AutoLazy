using System;
using AutoLazy.Autofixture;
using AutoLazy.Services;
using AutoFixture.NUnit3;
using NUnit.Framework;
using Autofac;

namespace AutoLazy
{
    [TestFixture,Parallelizable(ParallelScope.Self)]
    public class AutofacCircularDependencyIntegrationTests
    {
        [Test,AutoData]
        public void Resolving_a_component_which_depends_upon_a_circular_dependency_does_not_throw_exception_when_it_is_AutoLazy([TestingContainer] IContainer container)
        {
            using (var scope = container.BeginLifetimeScope(MakeServiceWithCircularDependency2AutoLazy))
            {
                Assert.That(() => scope.Resolve<IDependsOnCircularDependency>(), Throws.Nothing);
            }
        }

        [Test, AutoData]
        public void Using_a_resolved_component_with_circular_dependency_uses_real_implementation_when_it_is_AutoLazy([TestingContainer] IContainer container)
        {
            using (var scope = container.BeginLifetimeScope(MakeServiceWithCircularDependency2AutoLazy))
            {
                // The real implementation of the services behind this dependency will return "Foo Bar"
                Assert.That(() => scope.Resolve<IDependsOnCircularDependency>().GetValue(), Is.EqualTo("Foo Bar"));
            }
        }

        [Test, AutoData]
        public void Resolving_a_component_where_every_interface_is_AutoLazy_does_not_throw_exception([TestingContainer] IContainer container)
        {
            using (var scope = container.BeginLifetimeScope(MakeAllDependenciesAutoLazy))
            {
                Assert.That(() => scope.Resolve<IDependsOnCircularDependency>(), Throws.Nothing);
            }
        }

        [Test, AutoData]
        public void Resolving_a_component_with_property_injection_sets_instance_to_property([TestingContainer] IContainer container)
        {
            using (var scope = container.BeginLifetimeScope(MakeAllDependenciesAutoLazy))
            {
                Assert.That(() => scope.Resolve<ServiceWhichDependsOnCircularDependency>().DependencyProperty, Is.Not.Null);
            }
        }

        [Test, AutoData]
        public void Resolving_a_component_without_property_injection_does_not_set_property([TestingContainer] IContainer container)
        {
            using (var scope = container.BeginLifetimeScope(MakeAllDependenciesAutoLazyWithoutPropertyInjection))
            {
                Assert.That(() => scope.Resolve<ServiceWhichDependsOnCircularDependency>().DependencyProperty, Is.Null);
            }
        }

        [Test, AutoData]
        public void Resolving_a_component_which_depends_upon_a_circular_dependency_does_not_throw_exception_when_the_component_gets_dependencies_lazily([TestingContainer] IContainer container)
        {
            using (var scope = container.BeginLifetimeScope(MakeConsumerGetAutoLazyDependenciesWithPropertyInjection))
            {
                Assert.That(() => scope.Resolve<IDependsOnCircularDependency>(), Throws.Nothing);
            }
        }

        [Test, AutoData]
        public void Resolving_a_component_which_is_marked_to_consume_dependencies_lazily_gets_a_property_value([TestingContainer] IContainer container)
        {
            using (var scope = container.BeginLifetimeScope(MakeConsumerGetAutoLazyDependenciesWithPropertyInjection))
            {
                Assert.That(() => scope.Resolve<IDependsOnCircularDependency>()?.DependencyProperty, Is.Not.Null);
            }
        }

        [Test, AutoData]
        public void Resolving_a_component_which_is_marked_to_consume_dependencies_lazily_does_not_have_a_property_value_when_property_injection_is_disabled([TestingContainer] IContainer container)
        {
            using (var scope = container.BeginLifetimeScope(MakeConsumerGetAutoLazyDependenciesWithoutPropertyInjection))
            {
                Assert.That(() => scope.Resolve<IDependsOnCircularDependency>()?.DependencyProperty, Is.Null);
            }
        }

        void MakeServiceWithCircularDependency2AutoLazy(ContainerBuilder builder)
        {
            builder.MakeAutoLazyInterface<IServiceWithCircularDependency2>();
        }

        void MakeAllDependenciesAutoLazy(ContainerBuilder builder)
        {
            builder.MakeAutoLazyInterfaces(true,
                                           typeof(IServiceWithCircularDependency1),
                                           typeof(IServiceWithCircularDependency2),
                                           typeof(IDependsOnCircularDependency));
        }

        void MakeAllDependenciesAutoLazyWithoutPropertyInjection(ContainerBuilder builder)
        {
            builder.MakeAutoLazyInterfaces(false,
                                           typeof(IServiceWithCircularDependency1),
                                           typeof(IServiceWithCircularDependency2),
                                           typeof(IDependsOnCircularDependency));
        }

        void MakeConsumerGetAutoLazyDependenciesWithPropertyInjection(ContainerBuilder builder)
        {
            builder.MakeConsumedInterfacesAutoLazy<ServiceWhichDependsOnCircularDependency>(true);
        }

        void MakeConsumerGetAutoLazyDependenciesWithoutPropertyInjection(ContainerBuilder builder)
        {
            builder.MakeConsumedInterfacesAutoLazy<ServiceWhichDependsOnCircularDependency>();
        }
    }
}
