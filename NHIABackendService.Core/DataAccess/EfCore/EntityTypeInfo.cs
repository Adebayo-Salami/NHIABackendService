using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHIABackendService.Core.DataAccess.EfCore
{
    [ExcludeFromCodeCoverage]
    public class EntityTypeInfo
    {
        public EntityTypeInfo(Type entityType, Type declaringType)
        {
            EntityType = entityType;
            DeclaringType = declaringType;
        }

        public Type EntityType { get; }

        public Type DeclaringType { get; }
    }
}
