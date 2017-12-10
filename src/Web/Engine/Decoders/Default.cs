using System;
using System.Drawing;

namespace Web.Engine.Codecs.Decoders
{
    public class Default : DecoderBase
    {
        public Default() : base(new[] { ".*" })
        {
        }

        public override string TextContent(byte[] buffer, int? pageNumber) => null;

        public override int PageCount(byte[] buffer) => 1;

        public override byte[] CreateThumbnail(byte[] buffer, Size size, int pageNumber) => throw new NotImplementedException();

        public override bool AppliesTo(string extension) => true;
    }
}