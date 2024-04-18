using NHIABackendService.Core.ViewModels.Enums;
using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace NHIABackendService.Core.ViewModels
{
    public class ApiResponse
    {
        public ApiResponseCodes Code { get; set; }
        public string Description { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public bool HasErrors => Errors.Any();
    }

    public class ApiResponse<T> : ApiResponse
    {
        public ApiResponse(T data = default, string message = "", ApiResponseCodes codes = ApiResponseCodes.OK, int? totalCount = 0, params string[] errors)
        {
            Payload = data;
            Errors = errors.ToList();
            if (!errors.Any())
                Code = codes;
            else
                Code = codes == ApiResponseCodes.OK ? ApiResponseCodes.ERROR : codes;
            Description = message;
            TotalCount = totalCount ?? 0;
        }

        public T Payload { get; set; }
        public int TotalCount { get; set; }
        public string ResponseCode { get; set; }
    }
}
