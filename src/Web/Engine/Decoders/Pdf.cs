using Ghostscript.NET.Rasterizer;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Web.Engine.Codecs.Decoders
{
    public class Pdf : DecoderBase
    {
        public Pdf() : base(new[] { ".pdf" })
        {
        }

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

        public override Stream CreateThumbnail(Stream stream, Size size, int pageNumber)
        {
            const string ghostDllPath = @"C:\Development\DRS\src\Web\bin\gsdll64.dll"; //todo: banish the hardcode
            var version = new Ghostscript.NET.GhostscriptVersionInfo(new Version(0, 0, 0), ghostDllPath, string.Empty, Ghostscript.NET.GhostscriptLicense.GPL);

            using (var rasterizer = new GhostscriptRasterizer())
            {
                var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + ".pdf");

                try
                {
                    using (var fs = File.Create(path))
                    {
                        //todo: why is ghostscript requiring i save the pdf to file first??
                        stream.CopyTo(fs);
                        stream.Dispose();
                    }

                    rasterizer.Open(path, version, true);

                    using (var thumbnail = rasterizer.GetPage(200, 200, pageNumber))
                    {
                        using (var thumbnailStream = new MemoryStream())
                        {
                            thumbnail.Save(thumbnailStream, ImageFormat.Png);
                            thumbnail.Dispose();
                            thumbnailStream.Position = 0;

                            return ResizeAndCrop(thumbnailStream, size.Width, size.Height);
                        }
                    }
                }
                finally
                {
                    File.Delete(path);
                }
            }
        }
    }
}