using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Web.Engine.Codecs.Decoders
{
    public abstract class DecoderBase : IDecoder
    {
        private readonly string[] _supportedFileTypes;

        protected DecoderBase(string[] supportedFileTypes)
        {
            _supportedFileTypes = supportedFileTypes;
        }

        public abstract string TextContent(Stream stream, int? pageNumber);
        public abstract int PageCount(Stream stream);
        public abstract Stream CreateThumbnail(Stream stream, Size size, int pageNumber = 1);

        public virtual bool AppliesTo(string extension) => _supportedFileTypes.Contains(extension);

        public static Stream ResizeAndCrop(Stream input, int width, int height)
        {
            using (var image = SixLabors.ImageSharp.Image.Load(input))
            {
                image.Mutate(
                    ctx => ctx.Resize(
                        new ResizeOptions
                        {
                            Size = new SixLabors.Primitives.Size(width, height),
                            Mode = ResizeMode.Crop
                        }));

                var outStream = new MemoryStream();

                image.SaveAsPng(outStream);

                outStream.Position = 0;

                return outStream;
            }
        }
    }
}