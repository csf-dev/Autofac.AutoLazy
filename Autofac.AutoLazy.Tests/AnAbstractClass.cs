using System;
namespace Autofac.AutoLazy
{
    /// <summary>
    /// This abstract class is not valid to be AutoLazy, it will raise an exception when we try.
    /// </summary>
    public abstract class AnAbstractClass
    {
        public abstract int GetRandomNumber();
    }

    public class AbstractClassImpl : AnAbstractClass
    {
        public override int GetRandomNumber() => 4; // chosen by fair dice roll.
                                                    // guaranteed to be random.
    }
}
