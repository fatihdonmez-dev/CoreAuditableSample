using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MongoDB.Driver;
using CoreAuditableSample.Models.MongoModels;
using MongoDatabaseSettings = CoreAuditableSample.Models.MongoModels.MongoDatabaseSettings;

namespace CoreAuditableSample.Models.AuditModels
{
    public abstract class AuditableIdentityContext : IdentityDbContext
    {
        private readonly MongoDatabaseSettings _mongoSettings;

        public AuditableIdentityContext(IOptions<MongoDatabaseSettings> mongoSettingsAccessor, DbContextOptions options) : base(options)
        {
            _mongoSettings = mongoSettingsAccessor.Value;
        }

        public DbSet<Audit> AuditLogs { get; set; }

        public virtual int SaveChanges(string userId)
        {
            OnBeforeSaveChanges(userId);
            var result = base.SaveChanges();
            return result;
        }

        private void OnBeforeSaveChanges(string userId)
        {
            if (_mongoSettings == null)
                return;

            var server = new MongoClient(_mongoSettings.ConnectionString);
            var DB = server.GetDatabase(_mongoSettings.DatabaseName);
            var collection = DB.GetCollection<Audit>(_mongoSettings.CollectionName);

            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;
                var auditEntry = new AuditEntry(entry);
                auditEntry.TableName = entry.Entity.GetType().Name;
                auditEntry.UserId = userId;
                auditEntries.Add(auditEntry);
                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.AuditType = AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = AuditType.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            try
            {
                foreach (var auditEntry in auditEntries)
                {
                    //mongo
                    collection.InsertOneAsync(auditEntry.ToAudit());
                }
            }
            catch (Exception)
            {
                return;
            }

        }
    }
}
