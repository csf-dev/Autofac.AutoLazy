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
    /// An Autofac Module which will alter the container to always resolve instances of <typeparamref name="T"/>
    /// lazily.
    /// </para>
    /// <para>
    /// Instead of registering this module directly, use either one of the following methods (or an appropriate overload):
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="ContainerBuilderExtensions.MakeAutoLazyInterface{T}(ContainerBuilder, bool)"/></item>
    /// <item><see cref="ContainerBuilderExtensions.MakeAutoLazyInterfaces(ContainerBuilder, IEnumerable{Type}, bool)"/></item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This module may be registered many times, for many different types of <typeparamref name="T"/>.  It will
    /// perform its work for each unique interface-type registered.
    /// What this module will do is to alter the resolution process for the <typeparamref name="T"/>
    /// interface when it is resolved for a constructor parameter or for property-injection.
    /// In addition to any processes which already occur, the actual instance of the interface which is resolved
    /// will have been created by <see cref="IResolvesAutoLazyServices"/>, making it an auto-lazy service.
    /// </para>
    /// </remarks>
    public class MakeAutoLazyByResolvedTypeModule<T> : global::Autofac.Module where T : class
    {
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

            if(handlePropertyInjection)
                registration.Activated += AssignAutoLazyInstanceToAllApplicableProperties;
        }

        #region constructor injection

        void AddAutoLazyResolvedParameter(object sender, PreparingEventArgs e)
        {
            e.Parameters = e.Parameters.Union(new[] { new ResolvedParameter(ParameterPredicate, ResolveParameterValue) });
        }

        bool ParameterPredicate(ParameterInfo param, IComponentContext ctx)
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

        T ResolveParameterValue(ParameterInfo param, IComponentContext ctx) => ResolveAutoLazy(ctx);

        #endregion

        #region property injection

        void AssignAutoLazyInstanceToAllApplicableProperties(object sender, ActivatedEventArgs<object> e)
        {
            var properties = GetSettablePropertiesOfAutoLazyInterfaceTypeFromActivatedComponentInstance(e);
            foreach (var property in properties)
                SetPropertyValueToAutoLazyInstance(property, e);
        }

        /// <summary>
        /// From the <see cref="Type"/> of the <see cref="ActivatedEventArgs{T}.Instance"/>, get all of its properties
        /// which have a <see cref="PropertyInfo.PropertyType"/> equal to <typeparamref name="T"/> and which have a
        /// useable setter.
        /// </summary>
        /// <returns>The settable properties of the activated component instance, which are of the auto-lazy interface type.</returns>
        /// <param name="e">The Autofac activated event args, giving us access to the newly-activated component instance.</param>
        IEnumerable<PropertyInfo> GetSettablePropertiesOfAutoLazyInterfaceTypeFromActivatedComponentInstance(ActivatedEventArgs<object> e)
        {
            var type = e.Instance.GetType();
            return (from property in type.GetProperties()
                    where
                        property.PropertyType == typeof(T)
                        && property.CanWrite
                        && property.GetIndexParameters().Length == 0
                    select property)
                .ToList();
        }

        void SetPropertyValueToAutoLazyInstance(PropertyInfo property, ActivatedEventArgs<object> e)
            => property.SetValue(e.Instance, ResolveAutoLazy(e.Context), null);

        #endregion

        T ResolveAutoLazy(IComponentContext ctx)
            => ctx.Resolve<IResolvesAutoLazyServices>().ResolveAutoLazyService<T>(ctx);

        /// <summary>
        /// Initializes a new instance of the <see cref="MakeAutoLazyByResolvedTypeModule{T}"/> class.
        /// </summary>
        /// <param name="handlePropertyInjection">If set to <c>true</c> then property-injection is handled,
        /// by setting any settable properties (of type <typeparamref name="T"/>) of any resolved components
        /// to an auto-lazy instance of <typeparamref name="T"/>.  If <c>false</c> then the property-injection
        /// process (if any) will not be altered.</param>
        public MakeAutoLazyByResolvedTypeModule(bool handlePropertyInjection)
        {
            InterfaceDetector.AssertIsInterface<T>();
            this.handlePropertyInjection = handlePropertyInjection;
        }
    }
}
