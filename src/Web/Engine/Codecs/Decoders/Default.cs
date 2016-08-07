using System;

namespace Web.Engine.Codecs.Decoders
{
    public class Default : File
    {
        public override string ExtractContent(int? pageNumber)
        {
            return null;
        }

        public override int ExtractPageCount()
        {
            return 1;
        }

        public override byte[] ExtractThumbnail(int width, int? height, int pageNumber)
        {
            throw new NotImplementedException();
        }

        public static readonly string[] SupportedFileTypes = {".*"};

        public Default(byte[] buffer, DRSConfig config)
            : base(buffer, config)
        {
        }
    }
}