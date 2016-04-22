using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace Web.Engine.Extensions
{
    public static class ImageExtensions
    {
        private static RectangleF ResizeKeepAspect(Size currentDimensions, int maxWidth, int maxHeight)
        {
            var destX = 0f;
            var destY = 0f;

            float nPercent;

            var nPercentW = (float) maxWidth / currentDimensions.Width;
            var nPercentH = (float) maxHeight / currentDimensions.Height;
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = Convert.ToInt16((maxWidth -
                                         (currentDimensions.Width * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = Convert.ToInt16((maxHeight -
                                         (currentDimensions.Height * nPercent)) / 2);
            }

            var destWidth = (int) (currentDimensions.Width * nPercent);
            var destHeight = (int) (currentDimensions.Height * nPercent);

            return new RectangleF(destX, destY, destWidth, destHeight);
        }

        private static RectangleF ResizeWidth(Size currentDimensions, int maxWidth)
        {
            var nPercent = (float) maxWidth / currentDimensions.Width;

            var destWidth = (int) (currentDimensions.Width * nPercent);
            var destHeight = (int) (currentDimensions.Height * nPercent);

            return new RectangleF(0, 0, destWidth, destHeight);
        }

        public static Image ToFixedSize(this Image imgPhoto, int width, int? height)
        {
            return (height != null) ? imgPhoto.ToFixedSize(width, height.Value) : imgPhoto.ToFixedSize(width);
        }

        private static Image ToFixedSize(this Image imgPhoto, int width, int height)
        {
            var dimensions = ResizeKeepAspect(imgPhoto.Size, width, height);

            var bmPhoto = new Bitmap(width, height,
                PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                imgPhoto.VerticalResolution);

            using (var grPhoto = Graphics.FromImage(bmPhoto))
            {
                grPhoto.Clear(Color.White);
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(imgPhoto,
                    new RectangleF(dimensions.X, dimensions.Y, dimensions.Width, dimensions.Height),
                    new RectangleF(0, 0, imgPhoto.Width, imgPhoto.Height),
                    GraphicsUnit.Pixel);
            }

            imgPhoto.Dispose();

            return bmPhoto;
        }

        private static Image ToFixedSize(this Image imgPhoto, int width)
        {
            var dimensions = ResizeWidth(imgPhoto.Size, width);

            var bmPhoto = new Bitmap(width, (int) dimensions.Height,
                PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                imgPhoto.VerticalResolution);

            using (var grPhoto = Graphics.FromImage(bmPhoto))
            {
                grPhoto.Clear(Color.White);
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(imgPhoto,
                    new RectangleF(dimensions.X, dimensions.Y, dimensions.Width, dimensions.Height),
                    new RectangleF(0, 0, imgPhoto.Width, imgPhoto.Height),
                    GraphicsUnit.Pixel);
            }

            imgPhoto.Dispose();

            return bmPhoto;
        }

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