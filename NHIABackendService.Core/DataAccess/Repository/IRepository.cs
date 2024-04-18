using NHIABackendService.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NHIABackendService.Core.DataAccess.Repository
{
    public interface IRepository
    {
    }

    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class, IEntity<int>
    {
    }

    public interface IRepository<TEntity, TPrimaryKey> : IRepository where TEntity : class, IEntity<TPrimaryKey>
    {
        IQueryable<TEntity> GetAll();

        IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors);

        List<TEntity> GetAllList();

        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        Task<List<TEntity>> GetAllListAsync();

        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);

        T Query<T>(Func<IQueryable<TEntity>, T> queryMethod);

        TEntity Get(TPrimaryKey id);

        Task<TEntity> GetAsync(TPrimaryKey id);

        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity FirstOrDefault(TPrimaryKey id);

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity Load(TPrimaryKey id);

        TEntity Insert(TEntity entity);

        Task<TEntity> InsertAsync(TEntity entity);

        TPrimaryKey InsertAndGetId(TEntity entity);

        Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity);

        TEntity InsertOrUpdate(TEntity entity);

        Task<TEntity> InsertOrUpdateAsync(TEntity entity);

        TPrimaryKey InsertOrUpdateAndGetId(TEntity entity);

        Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity);

        TEntity Update(TEntity entity);

        TEntity Update(TPrimaryKey id, Action<TEntity> updateAction);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task> updateAction);

        void Delete(TEntity entity);

        void Delete(TPrimaryKey id);

        void Delete(Expression<Func<TEntity, bool>> predicate);

        Task DeleteAsync(TEntity entity);

        Task DeleteAsync(TPrimaryKey id);

        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        int Count();

        int Count(Expression<Func<TEntity, bool>> predicate);

        Task<int> CountAsync();

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        long LongCount();

        long LongCount(Expression<Func<TEntity, bool>> predicate);

        Task<long> LongCountAsync();

        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
