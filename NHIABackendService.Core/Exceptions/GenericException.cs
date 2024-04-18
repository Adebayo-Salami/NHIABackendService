using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace NHIABackendService.Core.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class AppGenericException : Exception
    {
        public AppGenericException()
        {

        }

        public AppGenericException(string message) : base(message)
        {
        }

        public AppGenericException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        protected AppGenericException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public string ErrorCode { get; set; }
    }
}
