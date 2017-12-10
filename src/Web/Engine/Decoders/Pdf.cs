using Ghostscript.NET.Rasterizer;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Web.Engine.Extensions;

namespace Web.Engine.Codecs.Decoders
{
    public class Pdf : DecoderBase
    {
        public Pdf() : base(new[] { ".pdf" })
        {
        }

        public override string TextContent(byte[] buffer, int? pageNumber)
        {
            using (var reader = new PdfReader(buffer))
            {
                if (pageNumber != null)
                {
                    return PdfTextExtractor.GetTextFromPage(reader, pageNumber.Value,
                        new SimpleTextExtractionStrategy());
                }

                using (var output = new StringWriter())
                {
                    for (var i = 1; i <= reader.NumberOfPages; i++)
                    {
                        output.WriteLine(PdfTextExtractor.GetTextFromPage(reader, i,
                            new SimpleTextExtractionStrategy()));
                    }

                    return output.ToString();
                }
            }
        }

        public override int PageCount(byte[] buffer)
        {
            using (var reader = new PdfReader(buffer))
            {
                return reader.NumberOfPages;
            }
        }

        public override byte[] CreateThumbnail(byte[] buffer, Size size, int pageNumber)
        {
            using (var rasterizer = new GhostscriptRasterizer())
            {
                using (var stream = new MemoryStream(buffer))
                {
                    rasterizer.Open(stream);

                    using (var thumbnail = rasterizer
                        .GetPage(200, 200, pageNumber)
                        .ToFixedSize(size.Width, size.Height))
                    {
                        using (var ms = new MemoryStream())
                        {
                            thumbnail.Save(ms, ImageFormat.Png);

                            return ms.ToArray();
                        }
                    }
                }
            }
        }
    }
}