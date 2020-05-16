using System;
namespace Autofac.AutoLazy
{
    public interface IServiceInterface
    {
        int GetNumber(string param1, string param2);

        string GetString();
    }
}
