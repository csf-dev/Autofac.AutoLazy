namespace AutoLazy
{
    /// <summary>
    /// An object which gets instances of a arbitrary types, which have no behaviour of their own (IE: stubs).
    /// </summary>
    public interface IGetsStubs
    {
        /// <summary>
        /// Gets a stub instance of the specified type.
        /// </summary>
        /// <returns>A stub object.</returns>
        /// <typeparam name="T">The type of the stub to create.</typeparam>
        T GetStub<T>() where T : class;
    }
}
