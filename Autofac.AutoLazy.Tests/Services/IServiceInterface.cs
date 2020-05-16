using System;
namespace Autofac.AutoLazy.Services
{
    public interface IServiceInterface
    {
        int GetNumber(string param1, string param2);

        string GetString();

        void DoTheThing();
    }
}
