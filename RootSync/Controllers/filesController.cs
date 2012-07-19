using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace www.Controllers
{
    public class filesController : Controller
    {
        //
        // GET: /files/


        public ActionResult Index(string path)
        {
           string url = Request.Url.ToString();
           
            try
            {


                string rootpath = www.Core.Utility.storagePath() + User.Identity.Name + "/";
                string FTPPath = rootpath;

                //first validate this user's folder exists, if not, we need to create it.
                if (!System.IO.Directory.Exists(FTPPath))
                {
                    System.IO.Directory.CreateDirectory(FTPPath);
                }


                //now check to see if we need to add a path
                if (!string.IsNullOrEmpty(path))
                {
                    if (path.StartsWith("/")) path = path.TrimStart(new char[] { '/' });
                    if (path.EndsWith("/")) path = path.TrimEnd(new char[] { '/' });
                }



                ViewBag.ShowGT = false;
                    
                if (System.IO.File.Exists(FTPPath + path))  //download the file
                {
                    string[] f = path.Split('/');
                    string filename = f[f.Length - 1];

                    return File(FTPPath + path, www.Core.Utility.getMimeFromFile(FTPPath + path), filename);
                }
                else //query the path after confirming it exists
                {
                    if (System.IO.Directory.Exists(FTPPath + path))
                        FTPPath += path;


                    IEnumerable<DirectoryInfo> dirs = SafeWalk.EnumerateDirectoriesSafe(new DirectoryInfo(FTPPath));
                    IEnumerable<FileInfo> files = SafeWalk.EnumerateFilesSafe(new DirectoryInfo(FTPPath));

                   
                    if (FTPPath == rootpath)
                    {
                        ViewBag.path = "";
                        ViewBag.FolderName = " Home";
                        ViewBag.ParentPath = "";
                    }
                    else
                    {
                        if (!FTPPath.EndsWith("/")) FTPPath += "/";


                        ViewBag.path = FTPPath.Replace(rootpath,"");

                        ViewBag.ShowGT = true;
                        string[] p = ViewBag.path.Split('/');
                        ViewBag.FolderName = p[p.Length - 2];
                        string parentpath = "";
                        for (int i = 0; i < p.Length-2; i++)
                        {
                            parentpath += p[i] + "/";
                        }
                        ViewBag.ParentPath = parentpath.TrimEnd('/');

                    }


                    return View(new object[] { dirs, files });
                }
            }
            catch (DirectoryNotFoundException exp)
            {
                //throw new FTPSalesFileProcessingException("Could not open the ftp directory", exp);
            }
            catch (IOException exp)
            {
                //throw new FTPSalesFileProcessingException("Failed to access directory", exp);
            }

            if (Request.IsAjaxRequest())
                return PartialView("_Index");
            else return View();
        }







    }




    public static class SafeWalk
    {
        public static IEnumerable<FileInfo> EnumerateFilesSafe(this DirectoryInfo dir, string filter = "*.*", SearchOption opt = SearchOption.TopDirectoryOnly)
        {
            var retval = Enumerable.Empty<FileInfo>();

            try { retval = dir.EnumerateFiles(filter); }
            catch { }

            if (opt == SearchOption.AllDirectories)
                retval = retval.Concat(dir.EnumerateDirectoriesSafe(opt: opt).SelectMany(x => x.EnumerateFilesSafe(filter, opt)));

            return retval;
        }

        public static IEnumerable<DirectoryInfo> EnumerateDirectoriesSafe(this DirectoryInfo dir, string filter = "*.*", SearchOption opt = SearchOption.TopDirectoryOnly)
        {
            var retval = Enumerable.Empty<DirectoryInfo>();

            try { retval = dir.EnumerateDirectories(filter); }
            catch { }

            if (opt == SearchOption.AllDirectories)
                retval = retval.Concat(retval.SelectMany(x => x.EnumerateDirectoriesSafe(filter, opt)));

            return retval;
        } 

    } 

}
