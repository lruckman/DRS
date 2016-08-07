using System;
using System.Text;

namespace Web.Engine.Codecs.Decoders
{
    public class Text : File
    {
        public override string ExtractContent(int? pageNumber)
        {
            return Encoding.UTF8.GetString(Buffer);
        }

        public override int ExtractPageCount()
        {
            return 1;
        }

        public override byte[] ExtractThumbnail(int width, int? height, int pageNumber)
        {
            throw new NotImplementedException();
        }

        public static readonly string[] SupportedFileTypes = {".txt"};

        public Text(byte[] buffer, DRSConfig config)
            : base(buffer, config)
        {
        }
    }
}