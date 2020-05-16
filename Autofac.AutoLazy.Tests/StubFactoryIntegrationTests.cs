using System;
using Autofac.AutoLazy.Autofixture;
using AutoFixture.NUnit3;
using Castle.DynamicProxy;
using Autofac.AutoLazy.Services;
using NUnit.Framework;

namespace Autofac.AutoLazy
{
    [TestFixture,Parallelizable]
    public class StubFactoryIntegrationTests
    {
        [Test, AutoData]
        public void GetStub_returns_object_instance([SingletonGenerator] IProxyGenerator generator)
        {
            var sut = new StubFactory(generator, () => new StubInterceptor());
            Assert.That(() => sut.GetStub<IServiceInterface>(), Is.Not.Null);
        }

        [Test, AutoData]
        public void GetStub_returns_which_returns_default_for_value_type([SingletonGenerator] IProxyGenerator generator,
                                                                         string param1,
                                                                         string param2)
        {
            var sut = new StubFactory(generator, () => new StubInterceptor());
            Assert.That(() => sut.GetStub<IServiceInterface>()?.GetNumber(param1, param2), Is.Zero);
        }

        [Test, AutoData]
        public void GetStub_returns_which_returns_null_for_reference_type([SingletonGenerator] IProxyGenerator generator)
        {
            var sut = new StubFactory(generator, () => new StubInterceptor());
            Assert.That(() => sut.GetStub<IServiceInterface>()?.GetString(), Is.Null);
        }

        [Test, AutoData]
        public void GetStub_does_not_throw_when_executing_a_void_method([SingletonGenerator] IProxyGenerator generator)
        {
            var sut = new StubFactory(generator, () => new StubInterceptor());
            Assert.That(() => sut.GetStub<IServiceInterface>()?.DoTheThing(), Throws.Nothing);
        }

        [Test, AutoData]
        public void GetStub_throws_exception_for_invalid_abstract_class([SingletonGenerator] IProxyGenerator generator)
        {
            var sut = new StubFactory(generator, () => new StubInterceptor());
            Assert.That(() => sut.GetStub<AnAbstractClass>(), Throws.InstanceOf<AutoLazyException>());
        }
    }
}
