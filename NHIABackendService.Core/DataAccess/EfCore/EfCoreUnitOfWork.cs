using Microsoft.EntityFrameworkCore;
using NHIABackendService.Core.DataAccess.EfCore.UnitOfWork;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace NHIABackendService.Core.DataAccess.EfCore
{
    [ExcludeFromCodeCoverage]
    public class EfCoreUnitOfWork : IUnitOfWork
    {
        public readonly DbContext _context;

        private bool _disposed;

        public EfCoreUnitOfWork(DbContext context)
        {
            _context = context;
        }

        public void BeginTransaction()
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            if (_context.Database.GetDbConnection().State != ConnectionState.Open)
                _context.Database.OpenConnection();

            _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            _context.ChangeTracker.DetectChanges();

            SaveChanges();
            _context.Database.CommitTransaction();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing) _context.Dispose();
            _disposed = true;
        }

        public void Rollback()
        {
            _context.Database.CurrentTransaction?.Rollback();
        }

        public virtual TDbContext GetOrCreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            return (TDbContext)_context;
        }
    }
}
