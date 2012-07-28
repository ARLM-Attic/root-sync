using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace rootsync.Business.Models {
    public class RootSyncCustomInitializer : IDatabaseInitializer<RootSyncContext> {

        #region IDatabaseInitializer<RootSyncContext> Members

        public void InitializeDatabase(RootSyncContext context) {

            //TODO: Right now, just delete any existing database and recreate
            //      this will obviously change in the near future. :)

            if (context.Database.Exists()) {
                context.Database.Delete();
            }
            context.Database.Create();

            //TODO: This would be a good time to setup any sample data
            //      Also, tables that need to be prepopulated ... (ie: US States, Role Definitions, etc.) indexes, etc.

        }

        #endregion

    }
}
