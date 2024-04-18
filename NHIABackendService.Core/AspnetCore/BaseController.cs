using log4net;
using Microsoft.AspNetCore.Mvc;
using NHIABackendService.Core.AspnetCore.Identity;
using NHIABackendService.Core.Extensions;
using NHIABackendService.Core.Utility;
using NHIABackendService.Core.ViewModels;
using NHIABackendService.Core.ViewModels.Enums;
using System.Runtime.CompilerServices;

#nullable disable

namespace NHIABackendService.Core.AspnetCore
{
    public class BaseController : ControllerBase
    {
        private readonly ILog _logger;

        public BaseController()
        {
            _logger = LogManager.GetLogger(typeof(BaseController));
        }

        protected UserPrincipal CurrentUser => new UserPrincipal(User);

        protected Guid UserId
        {
            get { return Guid.Parse(CurrentUser.Claims.FirstOrDefault(x => x.Type == CoreConstants.UserIdKey)?.Value); }
        }

        protected List<string> GetListModelErrors()
        {
            return ModelState.Values
                .SelectMany(x => x.Errors
                    .Select(ie => ie.ErrorMessage))
                .ToList();
        }

        protected IActionResult HandleError(Exception ex, string customErrorMessage = null)
        {
            _logger.Error(ex.StackTrace, ex);

            var rsp = new ApiResponse<string>();
            rsp.Code = ApiResponseCodes.ERROR;
#if DEBUG
            rsp.Description = $"Error: {ex?.InnerException?.Message ?? ex.Message} --> {ex?.StackTrace}";
            return Ok(rsp);
#else
            rsp.Description = customErrorMessage ?? "An error occurred while processing your request!";
            return Ok(rsp);
#endif
        }

        public IActionResult ApiResponse<T>(T data = default, string message = "", ApiResponseCodes codes = ApiResponseCodes.OK, int? totalCount = 0, params string[] errors)
        {
            var response = new ApiResponse<T>(data, message, codes, totalCount, errors);
            response.Description = message ?? response.Code.GetDescription();
            return Ok(response);
        }

        protected async Task<ApiResponse<T>> HandleApiOperationAsync<T>(Func<Task<ApiResponse<T>>> action, [CallerLineNumber] int lineNo = 0, [CallerMemberName] string method = "")
        {
            var apiResponse = new ApiResponse<T>
            {
                Code = ApiResponseCodes.OK
            };

            try
            {
                var methodResponse = await action.Invoke();

                apiResponse.ResponseCode = methodResponse.ResponseCode;
                apiResponse.Payload = methodResponse.Payload;
                apiResponse.TotalCount = methodResponse.TotalCount;
                apiResponse.Code = methodResponse.Code;
                apiResponse.Errors = methodResponse.Errors;
                apiResponse.Description = string.IsNullOrEmpty(apiResponse.Description)
                    ? methodResponse.Description
                    : apiResponse.Description;

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace, ex);
                apiResponse.Code = ApiResponseCodes.EXCEPTION;

#if DEBUG
                apiResponse.Description = $"Error: {ex?.InnerException?.Message ?? ex.Message} --> {ex?.StackTrace}";
#else
                apiResponse.Description = "An error occurred while processing your request!";
#endif
                apiResponse.Errors.Add(apiResponse.Description);
                return apiResponse;
            }
        }

        protected string GetModelStateValidationError()
        {
            string message = ModelState.Values.FirstOrDefault()?.Errors?.FirstOrDefault()?.ErrorMessage;
            return message;
        }
    }
}
