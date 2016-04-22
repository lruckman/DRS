using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.PlatformAbstractions;

namespace Web.Engine.Codecs.Decoders
{
    public class Text : File
    {
        protected override string ExtractContent(int? pageNumber)
        {
            return Encoding.UTF8.GetString(Buffer);
        }

        protected override int ExtractPageCount()
        {
            return 1;
        }

        protected override void ExtractThumbnail(Stream outputStream, int width, int? height, int pageNumber)
        {
            throw new NotImplementedException();
        }

        public static readonly string[] SupportedFileTypes = {".txt"};

        public Text(byte[] buffer, IApplicationEnvironment appEnvironment)
            : base(buffer, appEnvironment)
        {
        }
    }
}