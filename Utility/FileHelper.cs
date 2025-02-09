using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Utility
{
    public static class FileHelper
    {
        public static async Task SaveFileAsync(IFormFile img, string folderPath, string fileName)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await img.CopyToAsync(stream);
            }
        }
        public static void DeleteFile(string folderPath,string imgName)
        {
            var filePath = Path.Combine(folderPath, imgName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
