using System;
namespace AutoLazy.Services
{
    public interface IServiceWithCircularDependency2
    {
        string GetValue(int number);
    }

    public class CircularDependency2Impl : IServiceWithCircularDependency2
    {
        readonly IServiceWithCircularDependency1 dependency;

        public string GetValue(int number)
        {
            while (number < 5)
                number = dependency.GetNextNumber(number);

            return "Foo Bar";
        }

        public CircularDependency2Impl(IServiceWithCircularDependency1 dependency)
        {
            this.dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
        }
    }
}
