//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Http;

//using System.Net.Http;
//using System.Configuration;
//using System.IO;
//using www.Models;


//namespace www.Controllers
//{
//    public class apiController : ApiController
//    {
//        public string RootPath
//        {
//            get
//            {
//                accountModel usr = www.DataAccess.DAL.retAccount(Int32.Parse(User.Identity.Name));
//                return ConfigurationManager.AppSettings["path"] + usr.guid + "/";
//            }
//        }
        





//        /// <summary>
//        /// Return an enumerable fileinfo list of the specified path
//        /// </summary>
//        /// <param name="path">Relative directory path.</param>
//        /// <returns>iEnumerable list of Files</returns>
//        public IEnumerable<FileInfo> GetAllFiles(string path)
//        {
//            try
//            {
//                string FTPPath = RootPath;

//                //first validate this user's folder exists, if not, we need to create it.
//                if (!System.IO.Directory.Exists(FTPPath))
//                {
//                    System.IO.Directory.CreateDirectory(FTPPath);
//                }


//                //now check to see if we need to add a path
//                if (!string.IsNullOrEmpty(path))
//                {
//                    if (path.StartsWith("/")) path = path.TrimStart(new char[] { '/' });
//                    if (path.EndsWith("/")) path = path.TrimEnd(new char[] { '/' });
//                }

//                if (System.IO.Directory.Exists(FTPPath + path))
//                    FTPPath += path;

//                //return SafeWalk.EnumerateDirectoriesSafe(new DirectoryInfo(FTPPath)); //IEnumerable<DirectoryInfo> dirs
//                return SafeWalk.EnumerateFilesSafe(new DirectoryInfo(FTPPath)); //IEnumerable<FileInfo> files
//            }
//            catch (DirectoryNotFoundException exp)
//            {
//            }
//            catch (IOException exp)
//            {
//            }
            
//            return null;
//        }




//        /// <summary>
//        /// Return an enumerable DirectoryInfo list of the specified path
//        /// </summary>
//        /// <param name="path">Relative directory path.</param>
//        /// <returns>iEnumerable list of Directories</returns>
//        public IEnumerable<DirectoryInfo> GetAllDirectories(string path)
//        {
//            try
//            {
//                string FTPPath = RootPath;

//                //first validate this user's folder exists, if not, we need to create it.
//                if (!System.IO.Directory.Exists(FTPPath))
//                {
//                    System.IO.Directory.CreateDirectory(FTPPath);
//                }


//                //now check to see if we need to add a path
//                if (!string.IsNullOrEmpty(path))
//                {
//                    if (path.StartsWith("/")) path = path.TrimStart(new char[] { '/' });
//                    if (path.EndsWith("/")) path = path.TrimEnd(new char[] { '/' });
//                }

//                if (System.IO.Directory.Exists(FTPPath + path))
//                    FTPPath += path;

//                return SafeWalk.EnumerateDirectoriesSafe(new DirectoryInfo(FTPPath)); //IEnumerable<DirectoryInfo> dirs
//                //return SafeWalk.EnumerateFilesSafe(new DirectoryInfo(FTPPath)); //IEnumerable<FileInfo> files
//            }
//            catch (DirectoryNotFoundException exp)
//            {
//            }
//            catch (IOException exp)
//            {
//            }

//            return null;
//        }














//    }
//}
