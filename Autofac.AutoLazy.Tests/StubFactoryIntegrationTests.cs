﻿using System;
using Autofac.AutoLazy.Autofixture;
using AutoFixture.NUnit3;
using Castle.DynamicProxy;
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
        public void GetStub_returns_which_returns_null_for_reference_type([SingletonGenerator] IProxyGenerator generator,
                                                                          string param1,
                                                                          string param2)
        {
            var sut = new StubFactory(generator, () => new StubInterceptor());
            Assert.That(() => sut.GetStub<IServiceInterface>()?.GetString(), Is.Null);
        }
    }
}
