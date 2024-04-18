using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using NHIABackendService.Entities.Auditing;

namespace NHIABackendService.Entities
{
    [Table(nameof(Role))]
    public class Role : IdentityRole<Guid>, IHasCreationTime
    {
        public bool IsActive { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsDefaultRole { get; set; }
    }
}
