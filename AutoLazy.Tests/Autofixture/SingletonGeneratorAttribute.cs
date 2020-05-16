using System;
using System.Reflection;
using AutoFixture;
using AutoFixture.NUnit3;
using Castle.DynamicProxy;

namespace AutoLazy.Autofixture
{
    /// <summary>
    /// Applied to an <see cref="IProxyGenerator"/>, causes a singleton 'real' <see cref="ProxyGenerator"/>
    /// instance to be returned.
    /// </summary>
    public class SingletonGeneratorAttribute : CustomizeAttribute
    {
        public override ICustomization GetCustomization(ParameterInfo parameter)
            => new SingletonGeneratorCustomization();
    }

    public class SingletonGeneratorCustomization : ICustomization
    {
        static readonly IProxyGenerator singleton = new ProxyGenerator();

        public void Customize(IFixture fixture)
        {
            fixture.Inject(singleton);
        }
    }
}
