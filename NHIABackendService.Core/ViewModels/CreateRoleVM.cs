#nullable disable

namespace NHIABackendService.Core.ViewModels
{
    public class CreateRoleVM
    {
        public string Name { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}
