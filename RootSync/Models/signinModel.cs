using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Security;

namespace www.Models
{
    

    public class signinModel
    {
        [Required]
        [Display(Name ="Username / Email")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*\\.([a-z]{2,4})$", ErrorMessage = "Not a valid email")]
        public string Username { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Password { get; set; }
    }
}
