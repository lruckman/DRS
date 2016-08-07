using System.Drawing.Imaging;
using System.IO;
using Ghostscript.NET.Rasterizer;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Web.Engine.Extensions;

namespace Web.Engine.Codecs.Decoders
{
    public class Pdf : File
    {
        public override string ExtractContent(int? pageNumber)
        {
            using (var reader = new PdfReader(Buffer))
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

        public override int ExtractPageCount()
        {
            using (var reader = new PdfReader(Buffer))
            {
                return reader.NumberOfPages;
            }
        }

        public override byte[] ExtractThumbnail(int width, int? height, int pageNumber)
        {
            using (var rasterizer = new GhostscriptRasterizer())
            {
                using (var stream = new MemoryStream(Buffer))
                {
                    rasterizer.Open(stream);

                    using (var thumbnail = rasterizer
                        .GetPage(200, 200, pageNumber)
                        .ToFixedSize(width, height))
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

        public static readonly string[] SupportedFileTypes = {".pdf"};

        public Pdf(byte[] buffer, DRSConfig config)
            : base(buffer, config)
        {
        }
    }
}