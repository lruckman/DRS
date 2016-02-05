using System.Drawing.Imaging;
using System.IO;
using Ghostscript.NET.Rasterizer;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace Web.Engine.FileParsers
{
    public class Pdf : FileParser
    {
        public Pdf(byte[] buffer) : base(buffer)
        {
        }

        protected override string Content(int? pageNumber)
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

        protected override int NumberOfPages()
        {
            using (var reader = new PdfReader(Buffer))
            {
                return reader.NumberOfPages;
            }
        }

        protected override void SaveThumbnail(int pageNumber, int dpi, Stream outputStream)
        {
            using (var rasterizer = new GhostscriptRasterizer())
            {
                using (var stream = new MemoryStream(Buffer))
                {
                    rasterizer.Open(stream);

                    using (var thumbnail = rasterizer.GetPage(dpi, dpi, pageNumber))
                    {
                        thumbnail.Save(outputStream, ImageFormat.Png);
                        thumbnail.Dispose();
                    }
                }
            }
        }
    }
}