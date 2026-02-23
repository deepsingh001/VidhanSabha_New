using System;
using System.IO;
using System.Web;

namespace GlobalVidhanSabha.Helpers
{
    public static class ImageUploadHelper
    {
        public static string SaveImage(HttpPostedFile file, string folderPath)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            // Ensure folder exists
            string serverPath = HttpContext.Current.Server.MapPath(folderPath);

            if (!Directory.Exists(serverPath))
                Directory.CreateDirectory(serverPath);

            // Generate unique file name
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            string fullPath = Path.Combine(serverPath, fileName);

            file.SaveAs(fullPath);

            // return relative path (DB me save karne ke liye)
            return folderPath + fileName;
        }
    }
}