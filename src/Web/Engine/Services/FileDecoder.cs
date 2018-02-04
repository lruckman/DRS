using System;
using System.IO;
using System.Linq;
using Web.Engine.Codecs.Decoders;
using Image = Web.Engine.Codecs.Decoders.Image;

namespace Web.Engine.Services
{
    public class FileDecoder : IFileDecoder
    {
        private static IDecoder[] _decoders;

        public static void RegisterFileDecoders(IOcrEngine ocrEngine)
        {
            if (ocrEngine == null)
            {
                throw new ArgumentNullException(nameof(ocrEngine));
            }

            _decoders = new IDecoder[]
            {
                new Image(ocrEngine),
                new Pdf(),
                new Text(),
                new Default()
            };
        }

        /// <summary>
        ///     Returns a file parser for the file type passed in.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if filename == null</exception>
        public IDecoder Get(string filename)
        {
            var extension = Path.GetExtension(filename)?.ToLower();

            if (extension == null)
            {
                throw new ArgumentException(nameof(extension));
            }

            return _decoders.First(d => d.AppliesTo(extension));
        }
    }
}