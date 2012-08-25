using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace www.Models {
    public class FilesModel {
        public IEnumerable<DirectoryInfo> Directories { get; set; }
        public IEnumerable<FileInfo> Files { get; set; }
        public string RelativePath { get; set; }
    }
}