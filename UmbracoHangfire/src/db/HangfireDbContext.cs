using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbracoHangfire
{
    /// <summary>
    /// Storage of hangfire jobs that have run
    /// Available for use by jobs to store executions 
    /// since Hangfire does not keep a permanent record
    /// </summary>
    public class HangfireDbContext : DbContext
    {
        public HangfireDbContext() : base("umbracoDbDSN")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<HangfireDbContext, HangfireDbConfiguration>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Types().Configure(entity => entity.ToTable("GDM" + entity.ClrType.Name));
        }

        public DbSet<HangfireHistory> HangfireHistories { get; set; }
        public IQueryable<HangfireHistory> GetByHangfireId(string hangfireId)
        {
            IQueryable<HangfireHistory> result =
                HangfireHistories.Where(p => p.HangfireId == hangfireId)
                .OrderByDescending(p => p.StartTime);
            return result;
        }

        public void SaveHistory(string hangfireId, string message, DateTime startDate)
        {
            HangfireHistory history = HangfireHistory.Create(hangfireId, message, startDate);
            HangfireHistories.Add(history);
            SaveChanges();
        }
    }
}
