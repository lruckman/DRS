using System;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;

namespace Web.Engine.Codecs.Decoders
{
    public class Default : File
    {
        protected override string ExtractContent(int? pageNumber)
        {
            return null;
        }

        protected override int ExtractPageCount()
        {
            return 1;
        }

        protected override void ExtractThumbnail(Stream outputStream, int pageNumber, int dpi)
        {
            throw new NotImplementedException();
        }

        public static readonly string[] SupportedFileTypes = {".*"};

        public Default(byte[] buffer, IApplicationEnvironment appEnvironment)
            : base(buffer, appEnvironment)
        {
        }
    }
}