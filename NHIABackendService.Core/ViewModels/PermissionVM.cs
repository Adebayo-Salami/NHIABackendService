using NHIABackendService.Core.Extensions;
using NHIABackendService.Core.Permissions;

#nullable disable

namespace NHIABackendService.Core.ViewModels
{
    public class PermissionVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        public static implicit operator PermissionVM(Permission model)
        {
            return new PermissionVM
            {
                Id = (int)model,
                Name = model.ToString(),
                Description = model.GetDescription()
            };
        }
    }
}
