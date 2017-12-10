using System;
using System.Drawing;
using System.Text;

namespace Web.Engine.Codecs.Decoders
{
    public class Text : DecoderBase
    {
        public Text() : base(new[] { ".txt" })
        {
        }

        public override string TextContent(byte[] buffer, int? pageNumber) => Encoding.UTF8.GetString(buffer);

        public override int PageCount(byte[] buffer) => 1;

        public override byte[] CreateThumbnail(byte[] buffer, Size size, int pageNumber) => throw new NotImplementedException();
    }
}