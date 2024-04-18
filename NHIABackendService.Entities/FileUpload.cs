using NHIABackendService.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace NHIABackendService.Entities
{
    [Table(nameof(FileUpload))]
    public class FileUpload : GenericFullAuditedEntity<Guid, Guid?>
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string ContentType { get; set; }
    }
}
