using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web;
using System.Runtime.InteropServices;
using System.IO;

namespace www.Core
{
    public class Utility
    {
        /// <summary>
        /// convert a files long size value into readable format
        /// </summary>
        /// <param name="size">file size in bytes</param>
        /// <returns></returns>
        public static string FileSize(long size)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (size >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                size = size / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would 
            // show a single decimal place, and no space. 
            return String.Format("{0:0.##} {1}", size, sizes[order]);
        }




        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static System.UInt32 FindMimeFromData(
            System.UInt32 pBC,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
            System.UInt32 cbSize,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzMimeProposed,
            System.UInt32 dwMimeFlags,
            out System.UInt32 ppwzMimeOut,
            System.UInt32 dwReserverd
        );

        public static string getMimeFromFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename + " not found");

            try
            {
                byte[] buffer = new byte[256];
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    if (fs.Length >= 256)
                        fs.Read(buffer, 0, 256);
                    else
                        fs.Read(buffer, 0, (int)fs.Length);
                }
            
                System.UInt32 mimetype;
                FindMimeFromData(0, null, buffer, 256, null, 0, out mimetype, 0);
                System.IntPtr mimeTypePtr = new IntPtr(mimetype);
                string mime = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);
                return mime;
            }
            catch
            {
                return "unknown/unknown";
            }
        } 




    }
}
