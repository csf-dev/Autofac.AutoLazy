using System;
using Autofac.AutoLazy.Autofixture;
using Autofac.AutoLazy.Services;
using AutoFixture.NUnit3;
using NUnit.Framework;

namespace Autofac.AutoLazy
{
    [TestFixture,Parallelizable(ParallelScope.Self)]
    public class AutofacCircularDependencyIntegrationTests
    {
        [Test,AutoData]
        public void Resolving_a_component_which_depends_upon_a_circular_dependency_does_not_throw_exception([TestingContainer] IContainer container)
        {
            using (var scope = container.BeginLifetimeScope(ApplyAutoLazy))
            {
                Assert.That(() => scope.Resolve<IDependsOnCircularDependency>(), Throws.Nothing);
            }
        }

        [Test, AutoData]
        public void Using_a_resolved_component_with_circular_dependency_uses_real_implementation([TestingContainer] IContainer container)
        {
            using (var scope = container.BeginLifetimeScope(ApplyAutoLazy))
            {
                // The real implementation of the services behind this dependency will return "Foo Bar"
                Assert.That(() => scope.Resolve<IDependsOnCircularDependency>().GetValue(), Is.EqualTo("Foo Bar"));
            }
        }

        void ApplyAutoLazy(ContainerBuilder builder)
        {
            builder.MakeAutoLazyInterface<IServiceWithCircularDependency2>();
        }
    }
}
