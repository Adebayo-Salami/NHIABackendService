using System;
using System.Threading.Tasks;

namespace NHIABackendService.Core.DataAccess.EfCore.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();

        Task<int> SaveChangesAsync();

        void BeginTransaction();

        void Commit();

        void Rollback();
    }
}
