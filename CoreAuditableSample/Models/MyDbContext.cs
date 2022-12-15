using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CoreAuditableSample.Models.MongoModels;
using CoreAuditableSample.Models.AuditModels;

namespace CoreAuditableSample.Models
{
    public class MyDbContext : AuditableIdentityContext
    {
        public MyDbContext(IOptions<MongoDatabaseSettings> mongoSettingsAccessor, DbContextOptions<MyDbContext> options)
       : base(mongoSettingsAccessor, options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        //public DbSet<APIModel.APILink> APILink { get; set; }

    }
}
