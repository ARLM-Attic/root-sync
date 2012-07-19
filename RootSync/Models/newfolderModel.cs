using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace www.Models
{


    public class newfolderModel
    {
        [Required]
        [Display(Name = "Folder Name")]
        public string Name { get; set; }
    }
}
