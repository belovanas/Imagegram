using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Imagegram.RequestValidators
{
    public static class ImageValidator
    {
        public static bool IsValidImageFile(IFormFile file)
        {
            if (file.Length < 0) return false;

            // Check file extension to prevent security threats associated with unknown file types
            var permittedExtensions = new HashSet<string> {".png", ".jpg", ".bmp"};
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext)) return false;
            //
            // // Check if file size is greater than permitted limit
            // if (file.Length > _config.FileSize) // 6MB
            // {
            //     return false;
            // }

            return true;
        }
    }
}