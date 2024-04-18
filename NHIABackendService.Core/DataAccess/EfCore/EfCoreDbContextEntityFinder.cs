using Microsoft.EntityFrameworkCore;
using NHIABackendService.Core.Reflection;
using NHIABackendService.Entities.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NHIABackendService.Core.DataAccess.EfCore
{
    [ExcludeFromCodeCoverage]
    public static class EfCoreDbContextEntityFinder
    {
        public static IEnumerable<EntityTypeInfo> GetEntityTypeInfos(Type dbContextType)
        {
            return
                from property in dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>)) &&
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType.GenericTypeArguments[0],
                        typeof(IEntity<>))
                select new EntityTypeInfo(property.PropertyType.GenericTypeArguments[0], property.DeclaringType);
        }
    }
}
