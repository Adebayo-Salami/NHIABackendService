using Microsoft.EntityFrameworkCore;
using NHIABackendService.Entities.Common;
using System;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace NHIABackendService.Core.DataAccess.EfCore.Context
{
    [ExcludeFromCodeCoverage]
    public static class DbExtensions
    {
        public static bool IsDatabaseFkDeleteException(this DbUpdateException updateEx,
            out string foreignKeyErrorMessage)
        {
            foreignKeyErrorMessage = null;

            if (updateEx == null || updateEx.Entries.All(e => e.State != EntityState.Deleted))
                return false;

            var exception = (updateEx.InnerException ?? updateEx.InnerException?.InnerException) as SqlException;
            var errors = exception?.Errors.Cast<SqlError>();

            var errorMessages = new StringBuilder();

            if (errors != null)
                foreach (var exceptionError in errors.Where(e => e.Number == 547))
                {
                    errorMessages.AppendLine($"Message: {exceptionError.Message}");
                    errorMessages.AppendLine($"ErrorNumber: {exceptionError.Number}");
                    errorMessages.AppendLine($"LineNumber: {exceptionError.LineNumber}");
                    errorMessages.AppendLine($"Source: {exceptionError.Source}");
                    errorMessages.AppendLine($"Procedure: {exceptionError.Procedure}");
                }

            if (errorMessages.Length == 0) return false;

            foreignKeyErrorMessage = errorMessages.ToString();

            return true;
        }

        public static bool IsUpdateConcurrencyException(this DbUpdateException ex, out string properties)
        {
            properties = null;

            if (ex == null || ex.Entries.All(e => e.State != EntityState.Modified))
                return false;

            var errorMessages = new StringBuilder();

            foreach (var entry in ex.Entries)
                if (entry.Entity is IEntity)
                {
                    var proposedValues = entry.CurrentValues;
                    var databaseValues = entry.GetDatabaseValues();

                    foreach (var property in proposedValues.Properties)
                    {
                        var proposedValue = proposedValues[property];
                        var databaseValue = databaseValues[property];

                        errorMessages.AppendLine($"Entity: {entry.Metadata.Name}\tOld Value:" +
                                                 $" {databaseValue}\tNew Value{proposedValue}");
                    }
                }
                else
                {
                    throw new NotSupportedException(
                        "Don't know how to handle concurrency conflicts for "
                        + entry.Metadata.Name);
                }

            if (errorMessages.Length == 0) return false;

            properties = errorMessages.ToString();
            return true;
        }
    }
}
