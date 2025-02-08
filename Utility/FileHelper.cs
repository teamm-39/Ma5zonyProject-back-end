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
        /// <summary>
        /// حفظ الصورة في السيرفر
        /// </summary>
        /// <param name="img">الملف المرفوع</param>
        /// <param name="folderPath">المسار الذي سيتم حفظ الملف فيه</param>
        /// <param name="fileName">اسم الملف النهائي</param>
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
    }
}
