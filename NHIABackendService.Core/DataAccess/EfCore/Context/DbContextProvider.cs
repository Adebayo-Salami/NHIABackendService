using Microsoft.EntityFrameworkCore;
using NHIABackendService.Core.DataAccess.EfCore.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHIABackendService.Core.DataAccess.EfCore.Context
{
    public interface IDbContextProvider<out TDbContext> where TDbContext : DbContext
    {
        TDbContext GetDbContext();
    }

    [ExcludeFromCodeCoverage]
    public sealed class UnitOfWorkDbContextProvider<TDbContext> : IDbContextProvider<TDbContext> where TDbContext : DbContext
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnitOfWorkDbContextProvider(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public TDbContext GetDbContext()
        {
            return _unitOfWork.GetDbContext<TDbContext>();
        }
    }
}
