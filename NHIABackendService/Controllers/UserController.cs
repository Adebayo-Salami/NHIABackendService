using Microsoft.AspNetCore.Mvc;
using NHIABackendService.Core.AspnetCore;
using NHIABackendService.Core.Pagination;
using NHIABackendService.Core.ViewModels.Enums;
using NHIABackendService.Core.ViewModels;
using NHIABackendService.Services.Interface;
using NHIABackendService.Core.AspnetCore.Policy;
using NHIABackendService.Core.Permissions;

namespace NHIABackendService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedList<UserVM>>), 200)]
        public async Task<IActionResult> SearchUsers([FromQuery] SearchVM model)
        {
            try
            {
                var result = await _userService.SearchUsers(model);

                return ApiResponse(message: result.Message, codes: ApiResponseCodes.OK, data: result.Data,
                    totalCount: result.Data.TotalItemCount);
            }

            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedList<UserVM>>), 200)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var result = await _userService.GetAllUsers();

                return ApiResponse(message: result.Message, codes: ApiResponseCodes.OK, data: result.Data,
                    totalCount: result.Data.Count);
            }

            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<UserVM>), 200)]
        public async Task<IActionResult> CreateUser(CreateUserVM model)
        {
            if (model == null)
                return ApiResponse<string>(errors: "Empty payload");

            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse(GetModelStateValidationError(), message: null, codes: ApiResponseCodes.INVALID_REQUEST);

                var result = await _userService.CreateUser(model);

                if (result.HasError)
                    return ApiResponse<UserVM>(errors: result.GetErrorMessages().ToArray());
                return ApiResponse(message: "Successful", codes: ApiResponseCodes.OK, data: result.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPut]
        [RequiresPermission(Permission.USER_UPDATE)]
        [ProducesResponseType(typeof(ApiResponse<UserVM>), 200)]
        public async Task<IActionResult> UpdateUser(UpdateUserVM model)
        {
            if (model == null)
                return ApiResponse<string>(errors: "Empty payload");

            try
            {
                var result = await _userService.UpdateUser(model);

                if (result.HasError)
                    return ApiResponse<UserVM>(errors: result.GetErrorMessages().ToArray());
                return ApiResponse(message: "Successful", codes: ApiResponseCodes.OK, data: result.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpDelete("{userId}")]
        [RequiresPermission(Permission.USER_DELETE)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                var result = await _userService.DeleteUser(userId);

                if (result.HasError)
                    return ApiResponse<bool>(errors: result.GetErrorMessages().ToArray());
                return ApiResponse(message: "Successful", codes: ApiResponseCodes.OK, data: result.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}
