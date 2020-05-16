using System;
namespace Autofac.AutoLazy.Services
{
    public interface IServiceWithCircularDependency1
    {
        string GetValue();
        int GetNextNumber(int number);
    }

    public class CircularDependency1Impl : IServiceWithCircularDependency1
    {
        readonly IServiceWithCircularDependency2 dependency;

        public string GetValue() => dependency.GetValue(0);

        public int GetNextNumber(int number) => number + 1;

        public CircularDependency1Impl(IServiceWithCircularDependency2 dependency)
        {
            this.dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
        }
    }
}
