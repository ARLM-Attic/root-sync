using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace www.Models
{
    public class uploadfileModel { 
        public int FileSize { get; set; } 
        public string Filename { get; set; } 
        public string ContentType { get; set; } 
        public byte[] Contents { get; set; } 
    }

}