using NHIABackendService.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace NHIABackendService.Entities.Auditing
{
    [Serializable]
    public abstract class CreationAuditedEntity : CreationAuditedEntity<int>, IEntity
    {
    }

    [Serializable]
    public abstract class CreationAuditedEntity<TPrimaryKey> : Entity<TPrimaryKey>, ICreationAudited
    {
        protected CreationAuditedEntity()
        {
            CreationTime = DateTime.Now;
        }

        public virtual DateTime CreationTime { get; set; }
        public virtual long? CreatorUserId { get; set; }
    }

    [Serializable]
    public abstract class GenericCreationAuditedEntity<TPrimaryKey, TUserId> : Entity<TPrimaryKey>, IGenericCreationAudited<TUserId>
    {
        protected GenericCreationAuditedEntity()
        {
            CreationTime = DateTime.Now;
        }

        public virtual DateTime CreationTime { get; set; }
        public virtual TUserId CreatorUserId { get; set; }
    }

    [Serializable]
    public abstract class CreationAuditedEntity<TPrimaryKey, TUser> : CreationAuditedEntity<TPrimaryKey>, ICreationAudited<TUser> where TUser : IEntity<long>
    {
        [ForeignKey("CreatorUserId")]
        public virtual TUser CreatorUser { get; set; }
    }
}
