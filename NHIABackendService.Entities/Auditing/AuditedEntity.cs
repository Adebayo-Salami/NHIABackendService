using NHIABackendService.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace NHIABackendService.Entities.Auditing
{
    [Serializable]
    public abstract class AuditedEntity : AuditedEntity<int>, IEntity
    {
    }

    [Serializable]
    public abstract class AuditedEntity<TPrimaryKey> : CreationAuditedEntity<TPrimaryKey>, IAudited
    {
        public virtual DateTime? LastModificationTime { get; set; }
        public virtual long? LastModifierUserId { get; set; }
    }

    [Serializable]
    public abstract class GenericAuditedEntity<TPrimaryKey, TUserId> : GenericCreationAuditedEntity<TPrimaryKey, TUserId>, IGenericAudited<TUserId>
    {
        public virtual DateTime? LastModificationTime { get; set; }
        public virtual TUserId LastModifierUserId { get; set; }
    }

    [Serializable]
    public abstract class AuditedEntity<TPrimaryKey, TUser> : AuditedEntity<TPrimaryKey>, IAudited<TUser> where TUser : IEntity<long>
    {
        [ForeignKey("CreatorUserId")]
        public virtual TUser CreatorUser { get; set; }

        [ForeignKey("LastModifierUserId")]
        public virtual TUser LastModifierUser { get; set; }
    }
}
