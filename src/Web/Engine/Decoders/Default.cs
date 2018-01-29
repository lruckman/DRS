using System;
using System.Drawing;
using System.IO;

namespace Web.Engine.Codecs.Decoders
{
    public class Default : DecoderBase
    {
        public Default() : base(new[] { ".*" })
        {
        }

        public override string TextContent(Stream stream, int? pageNumber) => null;

        public override int PageCount(Stream stream) => 1;

        public override byte[] CreateThumbnail(Stream stream, Size size, int pageNumber) => throw new NotImplementedException();

        public override bool AppliesTo(string extension) => true;
    }
}