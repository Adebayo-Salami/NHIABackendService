using Microsoft.Extensions.DependencyInjection;
using NHIABackendService.Core.DataAccess.EfCore;
using NHIABackendService.Core.DataAccess.Repository;
using System.Diagnostics.CodeAnalysis;

namespace NHIABackendService.Core.Utility
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtentions
    {
        public static void RegisterGenericRepos(this IServiceCollection self, Type dbContextType)
        {
            var repositoryInterface = typeof(IRepository<>);
            var repositoryInterfaceWithPrimaryKey = typeof(IRepository<,>);
            var repositoryImplementation = typeof(EfCoreRepository<,>);
            var repositoryImplementationWithPrimaryKey = typeof(EfCoreRepository<,,>);

            foreach (var entityTypeInfo in EfCoreDbContextEntityFinder.GetEntityTypeInfos(dbContextType))
            {
                var primaryKeyType = EntityHelper.GetPrimaryKeyType(entityTypeInfo.EntityType);
                if (primaryKeyType == typeof(int))
                {
                    var genericRepositoryType = repositoryInterface.MakeGenericType(entityTypeInfo.EntityType);
                    var implType = repositoryImplementation.GetGenericArguments().Length == 1
                        ? repositoryImplementation.MakeGenericType(entityTypeInfo.EntityType)
                        : repositoryImplementation.MakeGenericType(entityTypeInfo.DeclaringType,
                            entityTypeInfo.EntityType);

                    self.AddTransient(genericRepositoryType, implType);
                }

                var genericRepositoryTypeWithPrimaryKey =
                    repositoryInterfaceWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType, primaryKeyType);
                var implTypeWithKey = repositoryImplementationWithPrimaryKey.GetGenericArguments().Length == 2
                    ? repositoryImplementationWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType, primaryKeyType)
                    : repositoryImplementationWithPrimaryKey.MakeGenericType(entityTypeInfo.DeclaringType,
                        entityTypeInfo.EntityType, primaryKeyType);

                self.AddTransient(genericRepositoryTypeWithPrimaryKey, implTypeWithKey);
            }
        }
    }
}
