using System;
using System.Reflection;
using Autofac;
using AutoFixture;
using AutoFixture.NUnit3;

namespace AutoLazy.Autofixture
{
    /// <summary>
    /// Gets an Autofac <see cref="IContainer"/> using a <see cref="CachingContainerProvider"/>
    /// instance, built with registrations suitable for use by all tests.
    /// </summary>
    public class TestingContainerAttribute : CustomizeAttribute
    {
        public override ICustomization GetCustomization(ParameterInfo parameter)
            => new TestingContainerCustomization();
    }

    public class TestingContainerCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<IContainer>(c => c.FromFactory((CachingContainerProvider provider) => provider.GetContainer()));
        }
    }
}
