using System;
using System.Drawing.Imaging;
using System.IO;
using Ghostscript.NET.Rasterizer;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Extensions.PlatformAbstractions;

namespace Web.Engine.Codecs.Decoders
{
    public class Pdf : File
    {
        protected override string ExtractContent(int? pageNumber)
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

        protected override int ExtractPageCount()
        {
            using (var reader = new PdfReader(Buffer))
            {
                return reader.NumberOfPages;
            }
        }

        protected override void ExtractThumbnail(Stream outputStream, int pageNumber, int dpi)
        {
            if (outputStream == null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

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

        public static readonly string[] SupportedFileTypes = {".pdf"};

        public Pdf(byte[] buffer, IApplicationEnvironment appEnvironment)
            : base(buffer, appEnvironment)
        {
        }
    }
}