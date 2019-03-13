using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbracoHangfire
{
    public class HangfireDbConfiguration : DbMigrationsConfiguration<HangfireDbContext>
    { 
        public HangfireDbConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

    }
}
