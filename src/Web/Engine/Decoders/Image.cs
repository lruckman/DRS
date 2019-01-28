using System;
using System.Drawing;
using System.IO;
using Web.Engine.Extensions;
using Web.Engine.Services;

namespace Web.Engine.Codecs.Decoders
{
    public class Image : DecoderBase
    {
        private readonly IOcrEngine _ocr;

        public Image(IOcrEngine ocr)
            : base(new[] {
            ".gif", ".jpg", ".jpe", "jpeg", ".jif", ".jfif", ".jfi",
            ".png", ".bmp", ".tiff", ".tif" })
        {
            _ocr = ocr ?? throw new ArgumentNullException(nameof(ocr));
        }

        public override string TextContent(Stream stream, int? pageNumber)
        {
            using (var image = new Bitmap(stream))
            {
                return _ocr.GetText(image);
            }
        }

        public override int PageCount(Stream stream) => 1;

        public override Bitmap CreateThumbnail(Stream stream, Size size, int pageNumber)
        {
            using (var image = new Bitmap(stream).ToFixedSize(size.Width, size.Height))
            {
                return new Bitmap(image);
            }
        }
    }
}