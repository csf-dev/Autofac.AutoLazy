using System;
namespace Autofac.AutoLazy.Services
{
    public interface IDependsOnCircularDependency
    {
        string GetValue();
    }

    public class ServiceWhichDependsOnCircularDependency : IDependsOnCircularDependency
    {
        readonly IServiceWithCircularDependency1 dependency;

        public IServiceWithCircularDependency2 DependencyProperty { get; set; }

        public string GetValue() => dependency.GetValue();

        public ServiceWhichDependsOnCircularDependency(IServiceWithCircularDependency1 dependency)
        {
            this.dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
        }
    }
}
