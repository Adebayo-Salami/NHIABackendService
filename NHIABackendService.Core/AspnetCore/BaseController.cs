using log4net;
using Microsoft.AspNetCore.Mvc;
using NHIABackendService.Core.AspnetCore.Identity;
using NHIABackendService.Core.Extensions;
using NHIABackendService.Core.Utility;
using NHIABackendService.Core.ViewModels;
using NHIABackendService.Core.ViewModels.Enums;

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
    }
}
