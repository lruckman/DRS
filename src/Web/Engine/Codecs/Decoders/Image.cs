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

        public override string ExtractContent(int? pageNumber)
        {
            var dataPath = Config.TessDataPath;

            if (!Directory.Exists(dataPath))
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

        public override int ExtractPageCount()
        {
            return 1;
        }

        public override byte[] ExtractThumbnail(int width, int? height, int pageNumber)
        {
            using (var memoryStream = new MemoryStream(Buffer))
            {
                using (var image = new Bitmap(memoryStream).ToFixedSize(width, height))
                {
                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, ImageFormat.Png);

                        return ms.ToArray();
                    }
                }
            }
        }
    }
}