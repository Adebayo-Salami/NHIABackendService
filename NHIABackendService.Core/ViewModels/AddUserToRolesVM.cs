#nullable disable

namespace NHIABackendService.Core.ViewModels
{
    public class AddUserToRoleVM
    {
        public Guid UserId { get; set; }
        public string RoleName { get; set; }
    }

    public class RemoveUserFromRoleVM
    {
        public Guid UserId { get; set; }
        public string RoleName { get; set; }
    }

    public class AddPermissionsToRoleVM
    {
        public string RoleName { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();
    }

    public class RemovePermissionsFromRoleVM
    {
        public string RoleName { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}
