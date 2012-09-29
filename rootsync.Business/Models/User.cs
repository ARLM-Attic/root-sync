using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace rootsync.Business.Models {
    public class User {
        [Key]
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
        
    }
}
