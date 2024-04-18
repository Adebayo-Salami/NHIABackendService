using NHIABackendService.Core.Timing;
using NHIABackendService.Entities.Auditing;
using NHIABackendService.Entities.Extensions;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace NHIABackendService.Core.DataAccess.EfCore
{
    [ExcludeFromCodeCoverage]
    public static class EntityAuditingHelper
    {
        public static void SetCreationAuditProperties(object entityAsObj, long? userId)
        {
            if (!(entityAsObj is IHasCreationTime entityWithCreationTime))
                return;

            if (entityWithCreationTime.CreationTime == default) entityWithCreationTime.CreationTime = Clock.Now;

            if (!(entityAsObj is ICreationAudited))
                return;

            if (!userId.HasValue)
                return;

            var entity = entityAsObj as ICreationAudited;
            if (entity.CreatorUserId != null)
                return;

            entity.CreatorUserId = userId;
        }

        public static void SetModificationAuditProperties(object entityAsObj, long? userId)
        {
            if (entityAsObj is IHasModificationTime)
                entityAsObj.As<IHasModificationTime>().LastModificationTime = Clock.Now;

            if (!(entityAsObj is IModificationAudited))
                return;

            var entity = entityAsObj.As<IModificationAudited>();

            if (userId == null)
            {
                entity.LastModifierUserId = null;
                return;
            }

            entity.LastModifierUserId = userId;
        }
    }
}
