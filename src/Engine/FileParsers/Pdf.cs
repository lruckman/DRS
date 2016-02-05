using System.IO;
using Ghostscript.NET.Rasterizer;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace Engine.FileParsers
{
    public class Pdf : FileParser
    {
        public Pdf(byte[] buffer) : base(buffer)
        {
        }

        protected override string Content(byte[] buffer, int? pageNumber)
        {
            using (var reader = new PdfReader(buffer))
            {
                if (pageNumber != null)
                {
                    return PdfTextExtractor.GetTextFromPage(reader, pageNumber.Value, new SimpleTextExtractionStrategy());
                }

                using (var output = new StringWriter())
                {
                    for (var i = 1; i <= reader.NumberOfPages; i++)
                    {
                        output.WriteLine(PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy()));
                    }

                    return output.ToString();
                }
            }
        }

        protected override int NumberOfPages(byte[] buffer)
        {
            using (var reader = new PdfReader(buffer))
            {
                return reader.NumberOfPages;
            }
        }

        protected override string Thumbnail(byte[] buffer, int pageNumber, int dpi, string fileName)
        {
            using (var rasterizer = new GhostscriptRasterizer())
            {
                using (var fileIn = new MemoryStream(buffer))
                {
                    rasterizer.Open(fileIn);

                    using (var thumbnail = rasterizer.GetPage(dpi, dpi, pageNumber))
                    {
                        thumbnail.Save(fileName);
                        thumbnail.Dispose();

                        return fileName;
                    }
                }
            }
        }
    }
}