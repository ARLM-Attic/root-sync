using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Configuration;

namespace rootsync.Business.Models {
    public class RootSyncContext : DbContext {

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }

        public RootSyncContext() : base(ConfigurationManager.ConnectionStrings["DB"].ConnectionString) {
            
        }

    }
}
