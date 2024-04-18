using System.ComponentModel.DataAnnotations;

#nullable disable

namespace NHIABackendService.Core.ViewModels
{
    public class CreateUserVM
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "User Type Options: 1 - Default")]
        public Entities.Enums.UserType UserType { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
