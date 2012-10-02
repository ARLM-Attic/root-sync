using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace rootsync.Business.Models {
    public class User {
        [Key]
        public int UserID { get; set; }

        public Guid guid { get; set; }
        
        [MaxLength(50)]
        public string Username { get; set; }
        
        [MaxLength]
        public string Password { get; set; }
        
        [MaxLength(50)]
        public string First { get; set; }
        
        [MaxLength(50)]
        public string Last { get; set; }

        public DateTime? FirstLogin { get; set; }
        public DateTime? LastLogin { get; set; }
        
        [DefaultValue(typeof(int), "0")]
        public int numLogins { get; set; }

        public static User GetByUserID(int userid) {
            using (RootSyncContext context = new RootSyncContext()) {
                return context.Users.FirstOrDefault(u => u.userid == userid);
            }
        }

        public static User GetUserByGUID(Guid guid) {
            using (RootSyncContext context = new RootSyncContext()) {
                return context.Users.FirstOrDefault(u => u.guid == guid);
            }
        }

    }
}
