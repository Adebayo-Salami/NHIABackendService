using NHIABackendService.Entities;

#nullable disable

namespace NHIABackendService.Core.ViewModels
{
    public class RoleVM
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public static implicit operator RoleVM(Role model)
        {
            return model == null
                ? null
                : new RoleVM
                {
                    Id = model.Id.ToString(),
                    Name = model.Name
                };
        }
    }
}
