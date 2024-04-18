using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NHIABackendService.Core.DataAccess.EfCore.UnitOfWork;

namespace NHIABackendService.Core.DataAccess.EfCore.Context
{
    [ExcludeFromCodeCoverage]
    public static class EfUnitOfWorkExtensions
    {
        public static TDbContext GetDbContext<TDbContext>(this IUnitOfWork unitOfWork) where TDbContext : DbContext
        {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));

            if (!(unitOfWork is EfCoreUnitOfWork))
                throw new ArgumentException("unitOfWork is not type of " + typeof(EfCoreUnitOfWork).FullName,
                    nameof(unitOfWork));

            return (unitOfWork as EfCoreUnitOfWork).GetOrCreateDbContext<TDbContext>();
        }
    }
}
