using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Threading;
using www.Models;
using System.Configuration;
using rootsync.Business.Models;
using rootsync.Business;

namespace www.Controllers
{
    public class fileactionsController : Controller {

        public ActionResult GetDirectoryList(string path) {

            rootsync.Business.Models.User usr = rootsync.Business.Models.User.GetByUserID(Int32.Parse(User.Identity.Name));

            RSDirectory rsdir = RSFileSystem.GetDirectory(path, usr.guid);
            string userRoot = RSFileSystem.GetUserRoot(Guid.Parse(User.Identity.Name));
            //string FTPPath = RootPath;
            //string foldername = Request["foldername"];

            //if (foldername == "..") {
            //    path = path.Substring(0, path.Length - 2);
            //    //Remove the last folder
            //    path = RSFileSystem.GetParentDirectory(path, 1);
            //}

            //if (System.IO.Directory.Exists(FTPPath + path)) { FTPPath += path; }
            

            IEnumerable<DirectoryInfo> dirs = SafeWalk.EnumerateDirectoriesSafe(new DirectoryInfo(rsdir.SystemPath));
            IEnumerable<FileInfo> files = SafeWalk.EnumerateFilesSafe(new DirectoryInfo(rsdir.SystemPath));


            if (rsdir.SystemPath == userRoot) {
                ViewBag.path = "";
                ViewBag.FolderName = " Home";
                ViewBag.ParentPath = "";
            } else {

                //TODO: replace this with a "safe" RSDirectory.GetRelativePath() which will return the user-visible path

                if (!rsdir.SystemPath.EndsWith("/")) rsdir.SystemPath += "/";


                ViewBag.path = rsdir.SystemPath.Replace(userRoot, "");

                ViewBag.ShowGT = true;
                string[] p = ViewBag.path.Split('/');
                ViewBag.FolderName = p[p.Length - 2];
                string parentpath = "";
                for (int i = 0; i < p.Length - 2; i++) {
                    parentpath += p[i] + "/";
                }
                ViewBag.ParentPath = parentpath.TrimEnd('/');

            }


            string relativePath = (path ?? "").EndsWith("/") ? path : path + "/";

            return View("~/Views/files/DirectoryList.cshtml", new FilesModel() { Directories = dirs, Files = files, RelativePath = relativePath });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Rename(FormCollection formValues) {

            string srcFile = formValues["source"];
            string destFile = formValues["destination"];
            return Json("rename " + srcFile + " to " + destFile);
        }


        //
        // GET: /files/

        public ActionResult Index(string path)
        {
            if (string.IsNullOrEmpty(path)) path = "";
            return Redirect(string.Format("{0}://{1}/files/{2}", Request.Url.Scheme, Request.Url.Authority,path));
        }
        



        List<uploadfileModel> Uploads = new List<uploadfileModel>();
                
       
        public ActionResult UploadFile(string path)
        {
            ViewBag.path = path;

            uploadfileModel file = RetrieveFileFromRequest();

            if (file.Filename != null && !Uploads.Any(f => f.Filename.Equals(file.Filename)))
            {
                //Uploads.Add(file);
                
                var fileName = System.IO.Path.GetFileName(file.Filename);

                string FTPPath = RootPath;

                //first validate this user's folder exists, if not, we need to create it.
                if (!System.IO.Directory.Exists(FTPPath)) System.IO.Directory.CreateDirectory(FTPPath);

                //append new folder name and validate, create if it doesn't exist
                if (!string.IsNullOrEmpty(path))
                {
                    if (path.StartsWith("/")) path = path.TrimStart(new char[] { '/' });
                    if (!path.EndsWith("/") && path != "") path += "/";
                    FTPPath += path + fileName;
                }
                else { FTPPath += fileName; }





                var bw = new BinaryWriter(System.IO.File.Open(FTPPath, FileMode.OpenOrCreate));
                bw.Write(file.Contents);
                bw.Close();

                //var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                //fileStream.Write(file.Contents, 0, file.FileSize);
                //fileStream.Close();


                return RedirectToAction("Index", new { path = path });
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_UploadFile");
            }
            return View();
        }


        public uploadfileModel RetrieveFileFromRequest()
        {
            string filename = null;
            string fileSize = null;
            string fileType = null;
            byte[] fileContents = null;

            if (Request.Files.Count > 0)
            {
                //they're uploading the old way        
                var file = Request.Files[0];
                fileContents = new byte[file.ContentLength];
                Request.Files[0].InputStream.Read(fileContents, 0, Request.Files[0].ContentLength);
                fileType = file.ContentType;
                filename = file.FileName;
            }
            else if (Request.ContentLength > 0)
            {
                fileContents = new byte[Request.ContentLength];
                Request.InputStream.Read(fileContents, 0, Request.ContentLength);
                filename = Request.Headers["X-File-Name"];
                fileSize = Request.Headers["X-File-Size"];
                fileType = Request.Headers["X-File-Type"];
            }

            return new uploadfileModel()
            {
                Filename = filename,
                ContentType = fileType,
                FileSize = fileContents != null ? fileContents.Length : 0,
                Contents = fileContents
            };
        }















        //[HttpGet]
        //public ActionResult UploadFile()
        //{
        //    if (Request.IsAjaxRequest())
        //    {
        //        return PartialView("_UploadFile");
        //    }
        //    return View();
        //}




        [HttpGet]
        public ActionResult NewFolder()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView("_NewFolder");
            }
            return View();
        }




        [HttpPost]
        public ActionResult NewFolder(newfolderModel model, string path)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return PartialView("_NewFolder", model);

                return View(model);
            }

            try
            {

                string FTPPath = RootPath;

                //first validate this user's folder exists, if not, we need to create it.
                if (!System.IO.Directory.Exists(FTPPath)) System.IO.Directory.CreateDirectory(FTPPath);


                //append new folder name and validate, create if it doesn't exist
                if (!string.IsNullOrEmpty(path))
                {
                    if (path.StartsWith("/")) path = path.TrimStart(new char[] { '/' });
                    if (!path.EndsWith("/") && path != "") path += "/";
                    FTPPath += path + model.Name;
                }
                else { FTPPath += model.Name; }

                
                if (!System.IO.Directory.Exists(FTPPath)) System.IO.Directory.CreateDirectory(FTPPath);


                //string location = string.Format("window.location = '{0}://{1}/fileactions/?path={2}", Request.Url.Scheme, Request.Url.Authority, path);
                string location = Request.Url.ToString().ToLower().Replace("/newfolder", "?path=" + path);
                //return JavaScript("window.location = '" + location + "'");
                return Json(new { status = "success" });

            }
            catch (Exception ex)
            {
                return Json(new { status = "failure", responseHTML = this.RenderPartialViewToString("~/Views/Error/AjaxError.cshtml", ex)});
                //ModelState.AddModelError("Password", "Invalid username or password");
                //return Json(new object[] { true, this.RenderPartialViewToString("_failure", model) });
            }

            //return JavaScript("location.reload(true)");
            
        }



    }




}
