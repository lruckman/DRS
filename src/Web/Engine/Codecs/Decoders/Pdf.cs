using System;
using System.Drawing.Imaging;
using System.IO;
using Ghostscript.NET.Rasterizer;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Hosting;
using Web.Engine.Extensions;

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

        protected override void ExtractThumbnail(Stream outputStream, int width, int? height, int pageNumber)
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

                    using (var thumbnail = rasterizer
                        .GetPage(200, 200, pageNumber)
                        .ToFixedSize(width, height))
                    {
                        thumbnail.Save(outputStream, ImageFormat.Png);
                    }
                }
            }
        }

        public static readonly string[] SupportedFileTypes = {".pdf"};

        public Pdf(byte[] buffer, IHostingEnvironment hostingEnvironment)
            : base(buffer, hostingEnvironment)
        {
        }
    }
}