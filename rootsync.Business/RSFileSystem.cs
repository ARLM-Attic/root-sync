using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace rootsync.Business {
    public class RSFileSystem {

        public static string GetUserRoot(Guid userGuid) {
            return Path.Combine(ConfigurationManager.AppSettings["path"].ToString(), userGuid.ToString());
        }

        public static RSDirectory GetDirectory(string path, Guid userGuid) {
            string userRoot = GetUserRoot(userGuid);
            if (String.IsNullOrWhiteSpace(path) || 
                (path.Length == 1 && (path[0] == Path.DirectorySeparatorChar || path[0] == Path.AltDirectorySeparatorChar))) {
                //Return the root
                return new RSDirectory(userRoot);
            }
            if (path == ".." || path == Path.AltDirectorySeparatorChar + ".." || path == Path.DirectorySeparatorChar + "..") {
                //Invalid
                throw new Exception("Can't go above root directory.");
            }

            //We now know the string is not the following:
            // ..
            // /..
            // \..
            // /
            // \
            //So it must start either with...
            //  a ".." followed by additional characters
            //  a directory name
            //  a / followed by a directory name

            //Remove any beginning directory separator chars. Path.Combine doesn't like them and we don't need them.
            while(path.StartsWith(Path.DirectorySeparatorChar.ToString()) || path.StartsWith(Path.AltDirectorySeparatorChar.ToString())) {
                path = path.Substring(1);
            }

            //Find each folder in the parameters that are passed through "path"

            if (path.EndsWith("..")) {
                path = path.Substring(0, path.Length - 2);
                //Remove the last folder
                path = RSFileSystem.GetParentDirectory(Path.Combine(userRoot, path), 1);
            } else {
                path = Path.Combine(userRoot, path);
            }
            List<string> directoryNames = new List<string>();
            DirectoryInfo currentDirectory = new DirectoryInfo(path);
            bool doneClimbing = false;
            do {
                directoryNames.Add(currentDirectory.Name);
                if (currentDirectory.Parent == null || currentDirectory.Parent.FullName == userRoot) { doneClimbing = true; }
                currentDirectory = currentDirectory.Parent;
            } while(!doneClimbing);

            directoryNames.Reverse();

            currentDirectory = null;
            string currentPath = userRoot;

            //Crawl up the tree until we find the directory desired.
            //If along the way, a directory isn't found, an exception will be thrown from "First()"
            //We only traverse directories we find as children starting from the user root.
            foreach (string dir in directoryNames) {
                string dx = Directory.GetDirectories(currentPath).First(d => new DirectoryInfo(d).Name.ToLower() == dir.ToLower());
                currentPath = Path.Combine(currentPath, dx);
            }

            return new RSDirectory(currentPath);

        }

        public static string GetParentDirectory(string path, int parentCount) {
            if (string.IsNullOrEmpty(path) || parentCount < 1)
                return path;

            string parent = System.IO.Path.GetDirectoryName(path);

            if (--parentCount > 0)
                return GetParentDirectory(parent, parentCount);

            return parent;
        }
    }

    public class RSDirectory {
        public string SystemPath { get; set; }
        public RSDirectory(string systemPath) {
            SystemPath = systemPath;
        }

        
    }
}
