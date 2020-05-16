using System;
namespace Autofac.AutoLazy
{
    public class ServiceImplementation : IServiceInterface
    {
        public static int ConstructionCount { get; set; }

        public int GetNumber(string param1, string param2)
            => (param1?.Length).GetValueOrDefault() + (param2?.Length).GetValueOrDefault();

        public string GetString() => ConstructionCount.ToString();

        public void DoTheThing() { /* No-op */ }

        public ServiceImplementation()
        {
            ConstructionCount++;
        }
    }
}
