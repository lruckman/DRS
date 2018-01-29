using System;
using System.Drawing;
using System.IO;
using Tesseract;
using Web.Engine.Extensions;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Web.Engine.Codecs.Decoders
{
    public class Image : DecoderBase
    {
        private readonly string _tesseractDataPath;

        public Image(string tesseractDataPath)
            : base(new[] {
            ".gif", ".jpg", ".jpe", "jpeg", ".jif", ".jfif", ".jfi",
            ".png", ".bmp", ".tiff", ".tif" })
        {
            if (!Directory.Exists(tesseractDataPath))
            {
                throw new ArgumentException("Path does not exist or access is denied.", nameof(tesseractDataPath));
            }

            _tesseractDataPath = tesseractDataPath;
        }

        public override string TextContent(Stream stream, int? pageNumber)
        {
            using (var engine = new TesseractEngine(_tesseractDataPath, "eng", EngineMode.Default)) // todo: make this injectable
            {
                // have to load Pix via a bitmap since Pix doesn't support loading a stream.
                using (var image = new Bitmap(stream))
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

        public override int PageCount(Stream stream) => 1;

        public override byte[] CreateThumbnail(Stream stream, Size size, int pageNumber)
        {
            using (var image = new Bitmap(stream).ToFixedSize(size.Width, size.Height))
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