using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace CH.CleanArchitecture.Infrastructure.Shared.Extensions
{
    public static class FileExtensions
    {
        public const int ImageMinimumBytes = 512;

        public static bool IsImage(this IFormFile postedFile) {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png") {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg") {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try {
                if (!postedFile.OpenReadStream().CanRead) {
                    return false;
                }
                //------------------------------------------
                //check whether the image size exceeding the limit or not
                //------------------------------------------ 
                if (postedFile.Length < ImageMinimumBytes) {
                    return false;
                }

                byte[] buffer = new byte[ImageMinimumBytes];
                postedFile.OpenReadStream().Read(buffer, 0, ImageMinimumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline)) {
                    return false;
                }
            }
            catch (Exception) {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try {
                using (var ms = new MemoryStream()) {
                    postedFile.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    ValidatePicture(fileBytes, postedFile.ContentType);
                }

            }
            catch (Exception) {
                return false;
            }
            finally {
                postedFile.OpenReadStream().Position = 0;
            }

            return true;
        }

        public static async Task<bool> IsImage(this IBrowserFile postedFile) {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png") {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.Name).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.Name).ToLower() != ".png"
                && Path.GetExtension(postedFile.Name).ToLower() != ".gif"
                && Path.GetExtension(postedFile.Name).ToLower() != ".jpeg") {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try {
                if (!postedFile.OpenReadStream().CanRead) {
                    return false;
                }
                //------------------------------------------
                //check whether the image size exceeding the limit or not
                //------------------------------------------ 
                if (postedFile.Size < ImageMinimumBytes) {
                    return false;
                }

                byte[] buffer = new byte[ImageMinimumBytes];
                await postedFile.OpenReadStream().ReadAsync(buffer, 0, ImageMinimumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline)) {
                    return false;
                }
            }
            catch (Exception) {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try {
                using (var ms = new MemoryStream()) {
                    await postedFile.OpenReadStream(Constants.MaxPhotoUploadSizeMb * 1024 * 1024).CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    ValidatePicture(fileBytes, postedFile.ContentType);
                }

            }
            catch (Exception) {
                return false;
            }
            finally {
                //await postedFile.OpenReadStream().Position = 0;
            }

            return true;
        }

        /// <summary>
        /// Validates input picture dimensions
        /// </summary>
        /// <param name="pictureBinary">Picture binary</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>Picture binary or throws an exception</returns>
        public static byte[] ValidatePicture(byte[] pictureBinary, string mimeType) {
            using var image = Image.Load<Rgba32>(pictureBinary, out var imageFormat);

            return EncodeImage(image, imageFormat);
        }

        private static byte[] EncodeImage<TPixel>(Image<TPixel> image, IImageFormat imageFormat, int? quality = null)
          where TPixel : unmanaged, IPixel<TPixel> {
            using var stream = new MemoryStream();
            var imageEncoder = Configuration.Default.ImageFormatsManager.FindEncoder(imageFormat);
            switch (imageEncoder) {
                case JpegEncoder jpegEncoder:
                    jpegEncoder.Encode(image, stream);
                    break;

                case PngEncoder pngEncoder:
                    pngEncoder.ColorType = PngColorType.RgbWithAlpha;
                    pngEncoder.Encode(image, stream);
                    break;

                case BmpEncoder bmpEncoder:
                    bmpEncoder.BitsPerPixel = BmpBitsPerPixel.Pixel32;
                    bmpEncoder.Encode(image, stream);
                    break;

                case GifEncoder gifEncoder:
                    gifEncoder.Encode(image, stream);
                    break;

                default:
                    imageEncoder.Encode(image, stream);
                    break;
            }

            return stream.ToArray();
        }
    }
}
