using System;
using System.Linq;
using System.Reflection;
using Autofac.Core;

namespace Autofac.AutoLazy
{
    public class MakeAutoLazyByResolvedTypeModule<T> : Module where T : class
    {
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            registration.Preparing += AddAutoLazyResolvedParameter;
        }

        void AddAutoLazyResolvedParameter(object sender, PreparingEventArgs e)
        {
            e.Parameters = e.Parameters.Union(new[] { new ResolvedParameter(GetParameterPredicate, ResolveParameterValue) });
        }

        bool GetParameterPredicate(ParameterInfo param, IComponentContext ctx)
        {
            return (param.ParameterType == typeof(T)
                    && !IsLazyProviderType(param?.Member?.DeclaringType));
        }

        bool IsLazyProviderType(Type type)
        {
            if (type == null) return false;
            var typeInfo = type.GetTypeInfo();

            if (!typeInfo.IsGenericType) return false;
            if (typeInfo.GetGenericTypeDefinition() == typeof(LazyInstanceProvider<>)) return true;
            return false;
        }

        T ResolveParameterValue(ParameterInfo param, IComponentContext ctx)
            => ctx.Resolve<IResolvesAutoLazyServices>().ResolveAutoLazyService<T>(ctx);

        public MakeAutoLazyByResolvedTypeModule()
        {
            InterfaceDetector.AssertIsInterface<T>();
        }
    }
}
