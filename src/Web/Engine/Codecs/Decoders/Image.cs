using System;
using System.Drawing;
using System.IO;
using Tesseract;
using Web.Engine.Extensions;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Web.Engine.Codecs.Decoders
{
    public class Image : File
    {
        public static readonly string[] SupportedFileTypes =
        {
            ".gif", ".jpg", ".jpe", "jpeg", ".jif", ".jfif", ".jfi",
            ".png", ".bmp", ".tiff", ".tif"
        };

        public Image(byte[] buffer, DRSConfig config)
            : base(buffer, config)
        {
        }

        protected override string ExtractContent(int? pageNumber)
        {
            var dataPath = Config.TessDataDirectory;

            if (!System.IO.File.Exists(dataPath))
            {
                throw new ArgumentException("Path does not exist or access is denied.", nameof(dataPath));
            }

            using (var engine = new TesseractEngine(dataPath, "eng", EngineMode.Default))
            {
                using (var memoryStream = new MemoryStream(Buffer))
                {
                    // have to load Pix via a bitmap since Pix doesn't support loading a stream.
                    using (var image = new Bitmap(memoryStream))
                    {
                        using (var pix = PixConverter.ToPix(image))
                        {
                            using (var page = engine.Process(pix))
                            {
                                return page.GetText();
                            }
                        }
                    }
                }
            }
        }

        protected override int ExtractPageCount()
        {
            return 1;
        }

        protected override void ExtractThumbnail(Stream outputStream, int width, int? height, int pageNumber)
        {
            if (outputStream == null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            using (var memoryStream = new MemoryStream(Buffer))
            {
                using (var image = new Bitmap(memoryStream).ToFixedSize(width, height))
                {
                    image.Save(outputStream, ImageFormat.Png);
                }
            }
        }
    }
}