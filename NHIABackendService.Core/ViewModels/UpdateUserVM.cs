#nullable disable

namespace NHIABackendService.Core.ViewModels
{
    public class UpdateUserVM
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public Guid UserId { get; set; }
    }
}
