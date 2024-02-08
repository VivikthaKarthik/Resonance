using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using System.Security.AccessControl;

namespace ResoClassAPI.Interceptors
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var dbcontext = eventData.Context;

            if (dbcontext is null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            try
            {
                List<string> ignoredColumns = new List<string>() { "Id", "CreatedOn", "CreatedBy", "ModifiedOn", "ModifiedBy" };
                var entries = dbcontext.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);
                string createdBy = "UNKNOWN";

                foreach (var entry in entries)
                {
                    string tableName = entry.Entity.GetType().Name;
                    var properties = entry.Properties.ToList();
                    if (properties.Any(x => x.Metadata.Name == "Id"))
                    {
                        long primaryKey = Convert.ToInt64(properties.Where(x => x.Metadata.Name == "Id").FirstOrDefault().OriginalValue);

                        if (properties.Any(x => x.Metadata.Name == "ModifiedBy"))
                            createdBy = properties.Where(x => x.Metadata.Name == "ModifiedBy").FirstOrDefault().OriginalValue.ToString();

                        foreach (var property in properties)
                        {
                            string columnName = property.Metadata.Name;
                            string originalValue = property.OriginalValue != null ? property.OriginalValue.ToString() : string.Empty;
                            string updatedValue = property.CurrentValue != null ? property.CurrentValue.ToString() : string.Empty;

                            if (!ignoredColumns.Contains(columnName) && originalValue != updatedValue)
                            {
                                var auditLog = new Audit
                                {
                                    RecordId = primaryKey,
                                    TableName = tableName,
                                    ColumnName = columnName,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = createdBy,
                                    OldValue = originalValue,
                                    NewValue = updatedValue
                                };
                                dbcontext.Add(auditLog);
                            }
                        }
                    }
                }
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
