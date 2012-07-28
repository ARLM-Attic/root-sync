using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace rootsync.Business.Models {
    public class User {
        [Key]
        public int UserID { get; set; }
        [MaxLength(50)]
        public string Username { get; set; }
        [MaxLength]
        public string Password { get; set; }
        [MaxLength(50)]
        public string First { get; set; }
        [MaxLength(50)]
        public string Last { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
