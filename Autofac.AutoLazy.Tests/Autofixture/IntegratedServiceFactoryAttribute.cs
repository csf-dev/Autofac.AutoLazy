using System;
using System.Reflection;
using AutoFixture;
using AutoFixture.NUnit3;
using Castle.DynamicProxy;

namespace Autofac.AutoLazy.Autofixture
{
    /// <summary>
    /// Fully constructs an integrated instance of <see cref="IGetsAutoLazyServices"/>, using
    /// <see cref="AutoLazyServiceFactory"/> and all of its dependencies.
    /// </summary>
    public class IntegratedServiceFactoryAttribute : CustomizeAttribute
    {
        public override ICustomization GetCustomization(ParameterInfo parameter)
            => new IntegratedServiceFactoryCustomization();
    }

    public class IntegratedServiceFactoryCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize(new SingletonGeneratorCustomization());
            fixture.Customize<IGetsAutoLazyServices>(c => c.FromFactory(IntegratedServiceFactoryFunc));
        }

        Func<IProxyGenerator, IGetsAutoLazyServices> IntegratedServiceFactoryFunc => GetIntegratedServiceFactory;

        IGetsAutoLazyServices GetIntegratedServiceFactory(IProxyGenerator generator)
        {
            var stubFactory = new StubFactory(generator, () => new StubInterceptor());
            return new AutoLazyServiceFactory(stubFactory, new LazyInterceptorFactory(), generator);
        }
    }
}
