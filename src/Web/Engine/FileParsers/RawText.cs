using System;
using System.IO;
using System.Text;

namespace Web.Engine.FileParsers
{
    public class RawText : FileParser
    {
        protected override string ExtractContent(int? pageNumber)
        {
            return Encoding.UTF8.GetString(Buffer);
        }

        protected override int ExtractNumberOfPages()
        {
            return 1;
        }

        protected override void ExtractThumbnail(Stream outputStream, int pageNumber, int dpi)
        {
            throw new NotImplementedException();
        }

        public static readonly string[] SupportedFileTypes = {".txt"};

        public RawText(byte[] buffer) : base(buffer)
        {
        }
    }
}