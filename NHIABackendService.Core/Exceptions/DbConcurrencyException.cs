using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System;

#nullable disable

namespace NHIABackendService.Core.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class AppDbConcurrencyException : Exception
    {
        public AppDbConcurrencyException()
        {
        }

        protected AppDbConcurrencyException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
        {
        }

        public AppDbConcurrencyException(string message)
            : base(message)
        {
        }

        public AppDbConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
