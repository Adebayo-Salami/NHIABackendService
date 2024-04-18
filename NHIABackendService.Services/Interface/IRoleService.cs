using NHIABackendService.Core.Pagination;
using NHIABackendService.Core.ViewModels;
using NHIABackendService.Services.Model;

namespace NHIABackendService.Services.Interface
{
    public interface IRoleService
    {
        ResultModel<PagedList<RoleVM>> GetRoles(QueryModel model);

        Task<ResultModel<PagedList<UserVM>>> GetUsersInRole(RoleRequestModel model);

        Task<ResultModel<string>> GetUserRole(Guid userId);

        ResultModel<IEnumerable<PermissionVM>> GetAllPermissions();

        Task<ResultModel<IEnumerable<PermissionVM>>> GetRolePermissions(string roleName);

        Task<ResultModel<RoleVM>> CreateRole(CreateRoleVM model);

        Task<ResultModel<RoleVM>> AddPermissionsToRole(AddPermissionsToRoleVM model);

        Task<ResultModel<RoleVM>> AddUserToRole(AddUserToRoleVM model);

        Task<ResultModel<RoleVM>> RemovePermissionsFromRole(RemovePermissionsFromRoleVM model);

        Task<ResultModel<bool>> RemoveUserFromRole(RemoveUserFromRoleVM model);
    }
}
