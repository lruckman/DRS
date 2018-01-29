using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Web.Engine.Codecs.Decoders
{
    public class Text : DecoderBase
    {
        public Text() : base(new[] { ".txt" })
        {
        }

        public override string TextContent(Stream stream, int? pageNumber)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public override int PageCount(Stream stream) => 1;

        public override byte[] CreateThumbnail(Stream stream, Size size, int pageNumber) => throw new NotImplementedException();
    }
}