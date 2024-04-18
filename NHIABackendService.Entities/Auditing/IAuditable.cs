using NHIABackendService.Entities.Common;

namespace NHIABackendService.Entities.Auditing
{
    public interface IHasCreationTime
    {
        DateTime CreationTime { get; set; }
    }

    public interface ICreationAudited : IHasCreationTime
    {
        long? CreatorUserId { get; set; }
    }

    public interface IGenericCreationAudited<TUserId> : IHasCreationTime
    {
        TUserId CreatorUserId { get; set; }
    }

    public interface ICreationAudited<TUser> : ICreationAudited where TUser : IEntity<long>
    {
        TUser CreatorUser { get; set; }
    }

    public interface IHasModificationTime
    {
        DateTime? LastModificationTime { get; set; }
    }

    public interface IModificationAudited : IHasModificationTime
    {
        long? LastModifierUserId { get; set; }
    }

    public interface IGenericModificationAudited<TUserId> : IHasModificationTime
    {
        TUserId LastModifierUserId { get; set; }
    }

    public interface IModificationAudited<TUser> : IModificationAudited where TUser : IEntity<long>
    {
        TUser LastModifierUser { get; set; }
    }

    public interface IHasDeletionTime : ISoftDelete
    {
        DateTime? DeletionTime { get; set; }
    }

    public interface IDeletionAudited : IHasDeletionTime
    {
        long? DeleterUserId { get; set; }
    }

    public interface IGenericDeletionAudited<TUserId> : IHasDeletionTime
    {
        TUserId DeleterUserId { get; set; }
    }

    public interface IDeletionAudited<TUser> : IDeletionAudited where TUser : IEntity<long>
    {
        TUser DeleterUser { get; set; }
    }

    public interface IAudited : ICreationAudited, IModificationAudited
    {
    }

    public interface IGenericAudited<TUserId> : IGenericCreationAudited<TUserId>, IGenericModificationAudited<TUserId>
    {
    }

    public interface IAudited<TUser> : IAudited, ICreationAudited<TUser>, IModificationAudited<TUser> where TUser : IEntity<long>
    {
    }

    public interface IFullAudited : IAudited, IDeletionAudited
    {
    }

    public interface IGenericFullAudited<TUserId> : IGenericAudited<TUserId>, IGenericDeletionAudited<TUserId>
    {
    }

    public interface IFullAudited<TUser> : IAudited<TUser>, IFullAudited, IDeletionAudited<TUser> where TUser : IEntity<long>
    {
    }
}
