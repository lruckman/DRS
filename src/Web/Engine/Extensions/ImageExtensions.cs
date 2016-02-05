using System;
using System.Drawing.Imaging;
using System.Linq;

namespace Web.Engine.Extensions
{
    public static class ImageExtensions
    {
        public static string FileExtensionFromEncoder(this ImageFormat format)
        {
            try
            {
                return ImageCodecInfo.GetImageEncoders()
                    .First(x => x.FormatID == format.Guid)
                    .FilenameExtension
                    .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                    .First()
                    .Trim('*')
                    .ToLower();
            }
            catch (Exception)
            {
                return "." + format.ToString().ToLower();
            }
        }
    }
}