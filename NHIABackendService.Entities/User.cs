using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using NHIABackendService.Entities.Auditing;
using NHIABackendService.Entities.Enums;

#nullable disable

namespace NHIABackendService.Entities
{
    [Table(nameof(User))]
    public class User : IdentityUser<Guid>, IHasCreationTime, IHasDeletionTime, IHasModificationTime
    {
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public bool IsFirstTimeLogin { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public UserType UserType { get; set; }
    }

    public class UserClaim : IdentityUserClaim<Guid>
    {
    }

    public class UserRole : IdentityUserRole<Guid>
    {
    }

    public class UserLogin : IdentityUserLogin<Guid>
    {
        public Guid Id { get; set; }
    }

    public class RoleClaim : IdentityRoleClaim<Guid>
    {
    }

    public class UserToken : IdentityUserToken<Guid>
    {
    }
}
