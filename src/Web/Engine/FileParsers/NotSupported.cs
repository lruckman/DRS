using System;
using System.IO;

namespace Web.Engine.FileParsers
{
    public class NotSupported : FileParser
    {
        protected override string ExtractContent(int? pageNumber)
        {
            return null;
        }

        protected override int ExtractNumberOfPages()
        {
            return 1;
        }

        protected override void ExtractThumbnail(Stream outputStream, int pageNumber, int dpi)
        {
            throw new NotImplementedException();
        }

        public static readonly string[] SupportedFileTypes = {".*"};

        public NotSupported(byte[] buffer) : base(buffer)
        {
        }
    }
}