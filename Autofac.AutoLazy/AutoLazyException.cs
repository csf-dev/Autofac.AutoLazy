using System;

namespace Autofac.AutoLazy
{
    /// <summary>
    /// Raised if an attempt is made to crete an invalid auto-lazy service.
    /// </summary>
#if !NETSTANDARD1_1
    [global::System.Serializable]
#endif
    public class AutoLazyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoLazyException"/> class
        /// </summary>
        public AutoLazyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoLazyException"/> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
        public AutoLazyException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoLazyException"/> class
        /// </summary>
        /// <param name="message">A <see cref="String"/> that describes the exception. </param>
        /// <param name="inner">The exception that is the cause of the current exception. </param>
        public AutoLazyException(string message, Exception inner) : base(message, inner)
        {
        }

#if !NETSTANDARD1_1
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoLazyException"/> class
        /// </summary>
        /// <param name="context">The contextual information about the source or destination.</param>
        /// <param name="info">The object that holds the serialized object data.</param>
        protected AutoLazyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}
