using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

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

        protected override void ExtractThumbnail(Stream outputStream, int width, int? height, int pageNumber)
        {
            throw new NotImplementedException();
        }

        public static readonly string[] SupportedFileTypes = {".*"};

        public Default(byte[] buffer, IHostingEnvironment hostingEnvironment)
            : base(buffer, hostingEnvironment)
        {
        }
    }
}