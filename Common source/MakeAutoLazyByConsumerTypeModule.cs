using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
#if AUTOFAC_5x
using Autofac.Core.Registration;
#endif

namespace AutoLazy.Autofac
{
    /// <summary>
    /// <para>
    /// An Autofac Module which will alter the container such that any dependencies of types which match a predicate
    /// will be resolved auto-lazily, provided those dependencies are of interface types.
    /// </para>
    /// <para>
    /// Instead of registering this module directly, use the following method (or an appropriate overload):
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="ContainerBuilderExtensions.MakeConsumedInterfacesAutoLazy{T}(ContainerBuilder, bool)"/></item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This module may be registered many times, each time with a different predicate by which to match
    /// dependency-consumers.  It will perform its work for each predicate registered.
    /// What this module will do is to alter the resolution process for the consumers when they are resolved,
    /// so that any constructor parameters (and optionally, settable properties) which are of interface types that
    /// can be resolved via Autofac, are resolved using an instance of <see cref="IResolvesAutoLazyServices"/>.
    /// </para>
    /// </remarks>
    public class MakeAutoLazyByConsumerTypeModule : global::Autofac.Module
    {
        readonly Func<Type, bool> consumerPredicate;
        readonly bool handlePropertyInjection;

        /// <summary>
        /// Override to attach module-specific functionality to a
        /// component registration.
        /// </summary>
        /// <remarks>This method will be called for all existing <i>and future</i> component
        /// registrations - ordering is not important.</remarks>
        /// <param name="componentRegistry">The component registry.</param>
        /// <param name="registration">The registration to attach functionality to.</param>
#if !AUTOFAC_5x
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
#else
        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
#endif
        {
            // Deals with constructor injection
            registration.Preparing += AddAutoLazyResolvedParameter;

            if (handlePropertyInjection)
                registration.Activated += AssignAutoLazyInstanceToAllApplicableProperties;
        }

        #region constructor injection

        void AddAutoLazyResolvedParameter(object sender, PreparingEventArgs e)
        {
            e.Parameters = e.Parameters.Union(new[] { new ResolvedParameter(ParameterPredicate, ResolveParameterValue) });
        }

        bool ParameterPredicate(ParameterInfo param, IComponentContext ctx)
        {
            var declaringType = param?.Member?.DeclaringType;

            return (declaringType != null
                    && consumerPredicate(declaringType)
                    && !IsLazyProviderType(declaringType)
                    && param.ParameterType.GetTypeInfo().IsInterface);
        }

        bool IsLazyProviderType(Type type)
        {
            if (type == null) return false;
            var typeInfo = type.GetTypeInfo();

            if (!typeInfo.IsGenericType) return false;
            if (typeInfo.GetGenericTypeDefinition() == typeof(LazyInstanceProvider<>)) return true;
            return false;
        }

        object ResolveParameterValue(ParameterInfo param, IComponentContext ctx)
            => ResolveAutoLazy(ctx, param.ParameterType);

        #endregion

        #region property injection

        void AssignAutoLazyInstanceToAllApplicableProperties(object sender, ActivatedEventArgs<object> e)
        {
            var instanceType = e.Instance.GetType();
            if (IsLazyProviderType(instanceType) || !consumerPredicate(instanceType))
                return;

            var properties = GetSettablePropertiesOfAutoLazyInterfaceTypeFromActivatedComponentInstance(e);
            foreach (var property in properties)
                SetPropertyValueToAutoLazyInstance(property, e);
        }

        /// <summary>
        /// From the <see cref="Type"/> of the <see cref="ActivatedEventArgs{T}.Instance"/>, get all of its properties
        /// which have a <see cref="PropertyInfo.PropertyType"/> which are interfaces which can be resolved by the
        /// <see cref="ActivatedEventArgs{T}.Context"/> and where the property has a useable setter.
        /// </summary>
        /// <returns>The settable properties of the activated auto-lazy component instance, which are of resolveable interface types.</returns>
        /// <param name="e">The Autofac activated event args, giving us access to the newly-activated component instance.</param>
        IEnumerable<PropertyInfo> GetSettablePropertiesOfAutoLazyInterfaceTypeFromActivatedComponentInstance(ActivatedEventArgs<object> e)
        {
            var type = e.Instance.GetType();
            return (from property in type.GetProperties()
                    where
                        property.PropertyType.GetTypeInfo().IsInterface
                        && e.Context.IsRegistered(property.PropertyType)
                        && property.CanWrite
                        && property.GetIndexParameters().Length == 0
                    select property)
                .ToList();
        }

        void SetPropertyValueToAutoLazyInstance(PropertyInfo property, ActivatedEventArgs<object> e)
            => property.SetValue(e.Instance, ResolveAutoLazy(e.Context, property.PropertyType), null);

        #endregion

        object ResolveAutoLazy(IComponentContext ctx, Type serviceType)
            => ctx.Resolve<IResolvesAutoLazyServices>().ResolveAutoLazyService(ctx, serviceType);

        /// <summary>
        /// <para>
        /// Initializes a new instance of the <see cref="MakeAutoLazyByConsumerTypeModule"/> class.
        /// </para>
        /// <para>
        /// Instead of using this constructor directly, use the following method (or an appropriate overload):
        /// </para>
        /// <list type="bullet">
        /// <item><see cref="ContainerBuilderExtensions.MakeConsumedInterfacesAutoLazy{T}(ContainerBuilder, bool)"/></item>
        /// </list>
        /// </summary>
        /// <param name="consumerPredicate">A predicate function which is used to determine
        /// which dependency-consumer types receive auto-lazy dependencies.</param>
        /// <param name="handlePropertyInjection">If set to <c>true</c> then property-injection is handled,
        /// by setting any settable properties on the resolved instance matching <paramref name="consumerPredicate"/>,
        /// where the property-type is a resolvable interface, to an auto-lazy instance.
        /// If <c>false</c> then the property-injection process (if any) will not be altered.</param>
        public MakeAutoLazyByConsumerTypeModule(Func<Type,bool> consumerPredicate,
                                                bool handlePropertyInjection)
        {
            this.consumerPredicate = consumerPredicate ?? throw new ArgumentNullException(nameof(consumerPredicate));
            this.handlePropertyInjection = handlePropertyInjection;
        }
    }
}
