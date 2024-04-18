using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using NHIABackendService.Core.Exceptions;
using NHIABackendService.Core.Timing;
using NHIABackendService.Core.Utility;
using NHIABackendService.Entities;
using NHIABackendService.Entities.Auditing;
using NHIABackendService.Entities.Common;
using NHIABackendService.Entities.Extensions;
using ReflectionHelper = NHIABackendService.Core.Reflection.ReflectionHelper;

#nullable disable

namespace NHIABackendService.Core.DataAccess.EfCore.Context
{
    [ExcludeFromCodeCoverage]
    public class BaseDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        private static readonly MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(BaseDbContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic);

        protected BaseDbContext(DbContextOptions options) : base(options)
        {
            InitializeDbContext();
        }

        private IGuidGenerator GuidGenerator { get; set; }

        private void InitializeDbContext()
        {
            SetNullsForInjectedProperties();
        }

        private void SetNullsForInjectedProperties()
        {
            GuidGenerator = SequentialGuidGenerator.Instance;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                ConfigureGlobalFiltersMethodInfo.MakeGenericMethod(entityType.ClrType).Invoke(this, new object[] { builder, entityType });
            }
        }

        protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
            where TEntity : class
        {
            if (entityType.BaseType != null || !ShouldFilterEntity<TEntity>(entityType)) return;
            var filterExpression = CreateFilterExpression<TEntity>();
            if (filterExpression != null) modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
        }

        protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity))) return true;

            return false;
        }

        protected virtual Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>() where TEntity : class
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (!typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity))) return expression;

            Expression<Func<TEntity, bool>> softDeleteFilter = e => !((ISoftDelete)e).IsDeleted;
            expression = softDeleteFilter;

            return expression;
        }

        public override int SaveChanges()
        {
            try
            {
                ApplyAbpConcepts();
                var result = base.SaveChanges();
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new AppDbConcurrencyException(ex.Message, ex);
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                ApplyAbpConcepts();
                var result = await base.SaveChangesAsync(cancellationToken);
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new AppDbConcurrencyException(ex.Message, ex);
            }
        }

        protected virtual void ApplyAbpConcepts()
        {
            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                ApplyAbpConcepts(entry, null);
            }
        }

        protected virtual void ApplyAbpConcepts(EntityEntry entry, long? userId)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ApplyAbpConceptsForAddedEntity(entry, userId);
                    break;

                case EntityState.Modified:
                    ApplyAbpConceptsForModifiedEntity(entry, userId);
                    break;

                case EntityState.Deleted:
                    ApplyAbpConceptsForDeletedEntity(entry, userId);
                    break;

                default:
                    break;
            }
        }

        protected virtual void ApplyAbpConceptsForAddedEntity(EntityEntry entry, long? userId)
        {
            CheckAndSetId(entry);
            SetCreationAuditProperties(entry.Entity, userId);
        }

        protected virtual void ApplyAbpConceptsForModifiedEntity(EntityEntry entry, long? userId)
        {
            SetModificationAuditProperties(entry.Entity, userId);
            if (entry.Entity is ISoftDelete && entry.Entity.As<ISoftDelete>().IsDeleted)
                SetDeletionAuditProperties(entry.Entity, userId);
        }

        protected virtual void ApplyAbpConceptsForDeletedEntity(EntityEntry entry, long? userId)
        {
            CancelDeletionForSoftDelete(entry);
            SetDeletionAuditProperties(entry.Entity, userId);
        }

        protected virtual void CheckAndSetId(EntityEntry entry)
        {
            //Set GUID Ids
            var entity = entry.Entity as IEntity<Guid>;
            if (entity == null || entity.Id != Guid.Empty) return;
            var dbGeneratedAttr = ReflectionHelper
                .GetSingleAttributeOrDefault<DatabaseGeneratedAttribute>(
                    entry.Property("Id").Metadata.PropertyInfo
                );

            if (dbGeneratedAttr == null || dbGeneratedAttr.DatabaseGeneratedOption == DatabaseGeneratedOption.None)
                entity.Id = GuidGenerator.Create();
        }

        protected virtual void SetCreationAuditProperties(object entityAsObj, long? userId)
        {
            EntityAuditingHelper.SetCreationAuditProperties(entityAsObj, userId);
        }

        protected virtual void SetModificationAuditProperties(object entityAsObj, long? userId)
        {
            EntityAuditingHelper.SetModificationAuditProperties(entityAsObj, userId);
        }

        protected virtual void CancelDeletionForSoftDelete(EntityEntry entry)
        {
            if (!(entry.Entity is ISoftDelete)) return;

            entry.Reload();
            entry.State = EntityState.Modified;
            entry.Entity.As<ISoftDelete>().IsDeleted = true;
        }

        protected virtual void SetDeletionAuditProperties(object entityAsObj, long? userId)
        {
            if (entityAsObj is IHasDeletionTime)
            {
                var entity = entityAsObj.As<IHasDeletionTime>();

                entity.DeletionTime ??= Clock.Now;
            }

            if (!(entityAsObj is IDeletionAudited)) return;

            var delEntity = entityAsObj.As<IDeletionAudited>();

            if (delEntity.DeleterUserId != null) return;
            delEntity.DeleterUserId = userId;
        }

        protected virtual Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expression1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _newValue;
            private readonly Expression _oldValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                return node == _oldValue ? _newValue : base.Visit(node);
            }
        }
    }
}
