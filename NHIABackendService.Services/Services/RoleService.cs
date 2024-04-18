using Microsoft.AspNetCore.Identity;
using NHIABackendService.Core.Caching;
using NHIABackendService.Core.DataAccess.EfCore.UnitOfWork;
using NHIABackendService.Core.Extensions;
using NHIABackendService.Core.Pagination;
using NHIABackendService.Core.Permissions;
using NHIABackendService.Core.Utility;
using NHIABackendService.Core.ViewModels;
using NHIABackendService.Entities;
using NHIABackendService.Services.Interface;
using NHIABackendService.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static NHIABackendService.Core.Utility.CoreConstants;

namespace NHIABackendService.Services.Services
{
    public class RoleService : IRoleService
    {
        private readonly ICacheManager _cacheManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public RoleService(UserManager<User> userManager, RoleManager<Role> roleManager, ICacheManager cacheManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _cacheManager = cacheManager;
            _unitOfWork = unitOfWork;
        }

        public ResultModel<IEnumerable<PermissionVM>> GetAllPermissions()
        {
            var permissions = (Enum.GetValues(typeof(Permission)) as Permission[])
                .Select(x => new PermissionVM
                {
                    Id = (int)x,
                    Name = x.ToString(),
                    Description = x.GetDescription()
                });

            return new ResultModel<IEnumerable<PermissionVM>>(permissions, "Success");
        }

        public async Task<ResultModel<IEnumerable<PermissionVM>>> GetRolePermissions(string roleName)
        {
            var resultModel = new ResultModel<IEnumerable<PermissionVM>>();

            var rolePermissions = await _cacheManager.AddOrGetAsync(
                $"{CacheConstants.Keys.RolePermissions}/{roleName.ToUpper()}",
                async () =>
                {
                    var role = await _roleManager.FindByNameAsync(roleName.ToUpper());
                    if (role == null)
                    {
                        resultModel.AddError("Role not found");
                        return new List<string>();
                    }

                    return (await _roleManager.GetClaimsAsync(role))
                        .Where(x => Enum.IsDefined(typeof(Permission), x.Value))
                        .Select(x => x.Value).ToList();
                }, CacheConstants.CacheTime.MonthInMinutes);

            if (resultModel.HasError)
                return resultModel;

            var permissions = rolePermissions.Select(x => (PermissionVM)Enum.Parse<Permission>(x));
            return new ResultModel<IEnumerable<PermissionVM>>(permissions, "Success");
        }

        public ResultModel<PagedList<RoleVM>> GetRoles(QueryModel model)
        {
            var roles = _roleManager.Roles;
            var pagedResult = (PagedList<Role>)roles.ToPagedList(model.PageIndex, model.PageSize);

            var result = new PagedList<RoleVM>(pagedResult.Items.Select(x => (RoleVM)x), model.PageIndex,
                model.PageSize, pagedResult.TotalItemCount);
            return new ResultModel<PagedList<RoleVM>>(result, "Success");
        }

        public async Task<ResultModel<string>> GetUserRole(Guid userId)
        {
            var resultModel = new ResultModel<string>();

            var role = await _cacheManager.AddOrGetAsync($"{CacheConstants.Keys.UserRole}/{userId.ToString()}",
                async () =>
                {
                    var user = await _userManager.FindByIdAsync(userId.ToString());
                    if (user == null)
                    {
                        resultModel.AddError("User not found");
                        return null;
                    }

                    return (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                }, CacheConstants.CacheTime.MonthInMinutes);

            if (resultModel.HasError)
                return resultModel;

            return new ResultModel<string>(role, "Success");
        }

        public async Task<ResultModel<PagedList<UserVM>>> GetUsersInRole(RoleRequestModel model)
        {
            var users = await _userManager.GetUsersInRoleAsync(model.RoleName.ToUpper());

            var pagedResult = (PagedList<User>)users.ToPagedList(model.PageIndex, model.PageSize);

            var result = new PagedList<UserVM>(pagedResult.Items.Select(x => (UserVM)x), model.PageIndex,
                model.PageSize, pagedResult.TotalItemCount);

            return new ResultModel<PagedList<UserVM>>(result, "Success");
        }

        public async Task<ResultModel<RoleVM>> AddPermissionsToRole(AddPermissionsToRoleVM model)
        {
            var role = await _roleManager.FindByNameAsync(model.RoleName.ToUpper());
            if (role == null)
                return new ResultModel<RoleVM>("Role not found");

            var existingPermissions = (await _roleManager.GetClaimsAsync(role))
                .Where(x => Enum.IsDefined(typeof(Permission), x.Value))
                .Select(x => (int)Enum.Parse<Permission>(x.Value));

            var addPermissionResult =
                await AddPermissionToRole(role, model.PermissionIds.Except(existingPermissions).ToList());
            if (addPermissionResult.HasError)
                return new ResultModel<RoleVM>(addPermissionResult.GetErrorMessages().FirstOrDefault());

            _cacheManager.Remove($"{CacheConstants.Keys.RolePermissions}/{role.Name}");

            return new ResultModel<RoleVM>(role, "Success");
        }

        public async Task<ResultModel<RoleVM>> AddUserToRole(AddUserToRoleVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
                return new ResultModel<RoleVM>("User not found");

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains(model.RoleName.ToUpper()))
                return new ResultModel<RoleVM>("User already has role");

            //Remove existing role if any
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            var addRoleResult = await _userManager.AddToRoleAsync(user, model.RoleName.ToUpper());
            if (!addRoleResult.Succeeded)
                return new ResultModel<RoleVM>(
                    $"failed to add role, {string.Join("; ", addRoleResult.Errors.Select(x => x.Description))}");

            _cacheManager.Remove($"{CacheConstants.Keys.UserRole}/{model.UserId.ToString()}");

            return new ResultModel<RoleVM>(new RoleVM { Name = model.RoleName }, "Success");
        }

        public async Task<ResultModel<RoleVM>> CreateRole(CreateRoleVM model)
        {
            _unitOfWork.BeginTransaction();

            var role = await _roleManager.FindByNameAsync(model.Name.ToUpper());

            if (role != null)
                return new ResultModel<RoleVM>("Role already exists");

            role = new Role
            {
                Name = model.Name.ToUpper(),
                IsActive = true
            };

            var createResult = await _roleManager.CreateAsync(role);

            if (!createResult.Succeeded)
                return new ResultModel<RoleVM>(createResult.Errors.ToString());

            var addPermissionResult = await AddPermissionToRole(role, model.PermissionIds);

            if (addPermissionResult.HasError)
                return new ResultModel<RoleVM>(addPermissionResult.GetErrorMessages().FirstOrDefault());

            _unitOfWork.Commit();

            return new ResultModel<RoleVM>(role, "Success");
        }

        public async Task<ResultModel<RoleVM>> RemovePermissionsFromRole(RemovePermissionsFromRoleVM model)
        {
            var role = await _roleManager.FindByNameAsync(model.RoleName.ToUpper());
            if (role == null)
                return new ResultModel<RoleVM>("Role not found");

            var permissions = model.PermissionIds.Select(x => (Permission)x);

            foreach (var permission in permissions)
            {
                var removePermissionResult = await _roleManager.RemoveClaimAsync(role,
                    new Claim(CoreConstants.Permission, permission.ToString()));
                if (!removePermissionResult.Succeeded)
                {
                    await _roleManager.DeleteAsync(role);
                    return new ResultModel<RoleVM>(removePermissionResult.Errors.FirstOrDefault().Description);
                }
            }

            _cacheManager.Remove($"{CacheConstants.Keys.RolePermissions}/{role.Name}");

            return new ResultModel<RoleVM>(role, "Success");
        }

        public async Task<ResultModel<bool>> RemoveUserFromRole(RemoveUserFromRoleVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
                return new ResultModel<bool>("User not found");

            var addRoleResult = await _userManager.RemoveFromRoleAsync(user, model.RoleName.ToUpper());
            if (!addRoleResult.Succeeded)
                return new ResultModel<bool>(
                    $"failed to remove role {model.RoleName.ToUpper()} from user {user.FullName}");

            _cacheManager.Remove($"{CacheConstants.Keys.UserRole}/{model.UserId.ToString()}");

            return new ResultModel<bool>(true, "Success");
        }

        private async Task<ResultModel<Role>> AddPermissionToRole(Role role, List<int> permissionIds)
        {
            var permissions = permissionIds.Where(x => Enum.IsDefined(typeof(Permission), x))
                .Select(x => (Permission)x);

            foreach (var permission in permissions)
            {
                var addRoleResult =
                    await _roleManager.AddClaimAsync(role, new Claim(CoreConstants.Permission, permission.ToString()));
                if (!addRoleResult.Succeeded)
                {
                    await _roleManager.DeleteAsync(role);
                    return new ResultModel<Role>(addRoleResult.Errors.FirstOrDefault().Description);
                }
            }

            return new ResultModel<Role>(role, "Success");
        }
    }
}
