﻿using NHIABackendService.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace NHIABackendService.Entities.Auditing
{
    [Serializable]
    public abstract class FullAuditedEntity : FullAuditedEntity<int>, IEntity
    {
    }

    [Serializable]
    public abstract class FullAuditedEntity<TPrimaryKey> : AuditedEntity<TPrimaryKey>, IFullAudited
    {
        public virtual bool IsDeleted { get; set; }
        public virtual long? DeleterUserId { get; set; }
        public virtual DateTime? DeletionTime { get; set; }
    }

    [Serializable]
    public abstract class GenericFullAuditedEntity<TPrimaryKey, TUserId> : GenericAuditedEntity<TPrimaryKey, TUserId>, IGenericFullAudited<TUserId>
    {
        public virtual bool IsDeleted { get; set; }
        public virtual TUserId DeleterUserId { get; set; }
        public virtual DateTime? DeletionTime { get; set; }
    }

    [Serializable]
    public abstract class FullAuditedEntity<TPrimaryKey, TUser> : AuditedEntity<TPrimaryKey, TUser>, IFullAudited<TUser> where TUser : IEntity<long>
    {
        public virtual bool IsDeleted { get; set; }
        [ForeignKey("DeleterUserId")]
        public virtual TUser DeleterUser { get; set; }
        public virtual long? DeleterUserId { get; set; }
        public virtual DateTime? DeletionTime { get; set; }
    }
}
