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

        void ApplyAutoLazy(ContainerBuilder builder)
        {

        }
    }
}
