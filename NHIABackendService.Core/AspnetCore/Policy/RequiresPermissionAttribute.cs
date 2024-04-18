using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using NHIABackendService.Core.Caching;
using NHIABackendService.Entities;
using static NHIABackendService.Core.Utility.CoreConstants;
using NHIABackendService.Core.Permissions;
using NHIABackendService.Core.Utility;
using NHIABackendService.Core.Exceptions;

namespace NHIABackendService.Core.AspnetCore.Policy
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiresPermissionAttribute : TypeFilterAttribute
    {
        public RequiresPermissionAttribute(params Permission[] permissions) : base(typeof(RequiresPermissionImplAttribute))
        {
            Arguments = new object[] { new PermissionsAuthorizationRequirement(permissions) };
        }

        public class RequiresPermissionImplAttribute : Attribute, IAsyncResourceFilter
        {
            private readonly IAuthorizationService _authService;

            private readonly ICacheManager _cacheManager;

            private readonly PermissionsAuthorizationRequirement _requiredPermissions;

            private readonly RoleManager<Role> _roleManager;

            private readonly UserManager<User> _userManager;

            public RequiresPermissionImplAttribute(IAuthorizationService authService, UserManager<User> userManager, RoleManager<Role> roleManager, ICacheManager cacheManager, PermissionsAuthorizationRequirement requiredPermissions)
            {
                _authService = authService;
                _requiredPermissions = requiredPermissions;
                _userManager = userManager;
                _roleManager = roleManager;
                _cacheManager = cacheManager;
            }

            public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
            {
                if (!(await _authService.AuthorizeAsync(context.HttpContext.User, context.ActionDescriptor.ToString(), _requiredPermissions)).Succeeded && !await CheckIfUserHasPermission(context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == CoreConstants.UserIdKey).Value, _requiredPermissions))
                {
                    context.Result = new ForbidResult(JwtBearerDefaults.AuthenticationScheme);
                    return;
                }

                await next();
            }

            private async Task<bool> CheckIfUserHasPermission(string userId, PermissionsAuthorizationRequirement requiredPermission)
            {
                try
                {
                    var permissions = await GetUserPermissions(userId);
                    if (requiredPermission.RequiredPermissions.All(x => permissions.Any(y => y == x.ToString())))
                        return true;
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            private async Task<string[]> GetUserPermissions(string userId)
            {
                User user = null;

                var userClaims = await _cacheManager.AddOrGetAsync($"{CacheConstants.Keys.UserClaims}/{userId}",
                    async () =>
                    {
                        user = await _userManager.FindByIdAsync(userId);
                        if (user == null)
                            throw new AppGenericException("User not found");

                        return await _userManager.GetClaimsAsync(user);
                    }, CacheConstants.CacheTime.MonthInMinutes);

                var userRole = await _cacheManager.AddOrGetAsync($"{CacheConstants.Keys.UserRole}/{userId}",
                    async () =>
                    {
                        if (user == null)
                            user = await _userManager.FindByIdAsync(userId);
                        if (user == null)
                            throw new AppGenericException("User not found");

                        return (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                    }, CacheConstants.CacheTime.MonthInMinutes);

                var rolePermissions = await _cacheManager.AddOrGetAsync(
                    $"{CacheConstants.Keys.RolePermissions}/{userRole?.ToUpper()}",
                    async () =>
                    {
                        var role = await _roleManager.FindByNameAsync(userRole?.ToUpper());
                        var roleClaims = await _roleManager.GetClaimsAsync(role);

                        return roleClaims.Where(x => x.Type == CoreConstants.Permission).Select(x => x.Value).ToList();
                    }, CacheConstants.CacheTime.MonthInMinutes);

                var userPermissions = new List<string>();
                userPermissions.AddRange(userClaims.Where(x => x.Type == CoreConstants.Permission)
                    .Select(x => x.Value));
                userPermissions.AddRange(rolePermissions);

                return userPermissions.ToArray();
            }
        }
    }
}
