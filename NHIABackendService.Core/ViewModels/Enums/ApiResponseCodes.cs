using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHIABackendService.Core.ViewModels.Enums
{
    public enum ApiResponseCodes
    {
        [Description("EXCEPTION ")]
        EXCEPTION = -1,

        [Description("Unauthorized Access")] 
        UNAUTHORIZED = -4,

        [Description("Not Found")] 
        NOT_FOUND = 401,

        [Description("Invalid Request")] 
        INVALID_REQUEST = 202,

        [Description("Server error occured, please try again.")]
        ERROR = 500,

        [Description("FAILED")] 
        FAILED = 201,

        [Description("SUCCESS")] 
        OK = 00
    }
}
