using System;
using AutoLazy.Autofixture;
using AutoFixture.NUnit3;
using AutoLazy.Services;
using NUnit.Framework;

namespace AutoLazy
{
    [TestFixture,NonParallelizable]
    public class AutoLazyServiceFactoryIntegrationTests
    {
        [Test, AutoData]
        public void GetAutoLazyService_returns_object_instance([IntegratedServiceFactory] IGetsAutoLazyServices sut)
        {
            var lazy = new Lazy<IServiceInterface>(() => new ServiceImplementation());
            Assert.That(() => sut.GetAutoLazyService(lazy),
                        Is.Not.Null);
        }

        [Test, AutoData]
        public void GetAutoLazyService_returns_object_of_correct_type([IntegratedServiceFactory] IGetsAutoLazyServices sut)
        {
            var lazy = new Lazy<IServiceInterface>(() => new ServiceImplementation());
            Assert.That(() => sut.GetAutoLazyService(lazy),
                        Is.InstanceOf<IServiceInterface>());
        }

        [Test, AutoData]
        public void GetAutoLazyService_does_not_construct_real_impl_when_not_used([IntegratedServiceFactory] IGetsAutoLazyServices sut)
        {
            ServiceImplementation.ConstructionCount = 0;
            var lazy = new Lazy<IServiceInterface>(() => new ServiceImplementation());

            sut.GetAutoLazyService(lazy);

            Assert.That(ServiceImplementation.ConstructionCount, Is.Zero);
        }

        [Test, AutoData]
        public void GetAutoLazyService_constructs_impl_once_after_first_usage([IntegratedServiceFactory] IGetsAutoLazyServices sut)
        {
            ServiceImplementation.ConstructionCount = 0;
            var lazy = new Lazy<IServiceInterface>(() => new ServiceImplementation());

            var service = sut.GetAutoLazyService(lazy);
            service.GetString();

            Assert.That(ServiceImplementation.ConstructionCount, Is.EqualTo(1));
        }

        [Test, AutoData]
        public void GetAutoLazyService_constructs_impl_only_once_for_many_usages([IntegratedServiceFactory] IGetsAutoLazyServices sut)
        {
            ServiceImplementation.ConstructionCount = 0;
            var lazy = new Lazy<IServiceInterface>(() => new ServiceImplementation());

            var service = sut.GetAutoLazyService(lazy);
            service.GetString();
            service.GetString();
            service.GetNumber(null, null);
            service.GetString();

            Assert.That(ServiceImplementation.ConstructionCount, Is.EqualTo(1));
        }

        [Test, AutoData]
        public void GetAutoLazyService_constructs_impl_again_for_different_autolazy_instance([IntegratedServiceFactory] IGetsAutoLazyServices sut)
        {
            ServiceImplementation.ConstructionCount = 0;
            var lazy1 = new Lazy<IServiceInterface>(() => new ServiceImplementation());
            var lazy2 = new Lazy<IServiceInterface>(() => new ServiceImplementation());

            var service1 = sut.GetAutoLazyService(lazy1);
            var service2 = sut.GetAutoLazyService(lazy2);
            service1.GetString();
            service2.GetString();

            Assert.That(ServiceImplementation.ConstructionCount, Is.EqualTo(2));
        }

        [Test, AutoData]
        public void GetAutoLazyService_uses_logic_from_lazy_impl_for_methods([IntegratedServiceFactory] IGetsAutoLazyServices sut)
        {
            var lazy = new Lazy<IServiceInterface>(() => new ServiceImplementation());

            var service = sut.GetAutoLazyService(lazy);

            // The logic in the service is to return a number equal to the lengths of both input strings, added together
            Assert.That(() => service.GetNumber("1234", "56789"), Is.EqualTo(9));
        }

        [Test, AutoData]
        public void GetAutoLazyService_throws_exception_if_used_with_abstract_class([IntegratedServiceFactory] IGetsAutoLazyServices sut)
        {
            var lazy = new Lazy<AnAbstractClass>(() => new AbstractClassImpl());
            Assert.That(() => sut.GetAutoLazyService(lazy), Throws.InstanceOf<AutoLazyException>());
        }

    }
}
