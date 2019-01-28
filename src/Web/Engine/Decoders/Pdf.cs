using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Drawing;
using System.IO;
using Web.Engine.Services;

namespace Web.Engine.Codecs.Decoders
{
    public class Pdf : DecoderBase
    {
        private readonly IPdfRasterizer _pdfRasterizer;

        public Pdf(IPdfRasterizer pdfRasterizer) : base(new[] { ".pdf" })
            => _pdfRasterizer = pdfRasterizer ?? throw new ArgumentNullException(nameof(pdfRasterizer));

        public override string TextContent(Stream stream, int? pageNumber)
        {
            using (var reader = new PdfReader(stream))
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

        public override int PageCount(Stream stream)
        {
            using (var reader = new PdfReader(stream))
            {
                return reader.NumberOfPages;
            }
        }

        public override Bitmap CreateThumbnail(Stream stream, Size size, int pageNumber)
            => _pdfRasterizer.GetThumbnail(stream, pageNumber);
    }
}