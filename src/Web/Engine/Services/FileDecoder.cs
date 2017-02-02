using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Microsoft.Extensions.Options;
using Web.Engine.Codecs.Decoders;
using Web.Engine.Extensions;
using Image = Web.Engine.Codecs.Decoders.Image;

namespace Web.Engine.Services
{
    public interface IFileDecoder
    {
        FileDecoder.IFileInfo Decode(string filename, byte[] buffer);
    }

    public class FileDecoder : IFileDecoder
    {
        public interface IFileInfo
        {
            int PageCount { get; }
            string Abstract { get; }
            string Content { get; }
            long Length { get; }
            byte[] CreateThumbnail(Size dimensions, int? pageNumber);
            byte[] Buffer { get; }
        }

        private class FileInfo : IFileInfo
        {
            private readonly IFile _decoder;
            private int _pageCount;
            private string _content;

            public int PageCount => _pageCount == 0 ? _pageCount = _decoder.ExtractPageCount() : _pageCount;
            public string Abstract => Content?.NormalizeLineEndings()?.Truncate(512);
            public string Content => string.IsNullOrWhiteSpace(_content) ? _content = _decoder.ExtractContent(null) : _content;

            public byte[] CreateThumbnail(Size dimensions, int? pageNumber)
            {
                return _decoder.ExtractThumbnail(dimensions.Width, dimensions.Height, pageNumber ?? 1);
            }

            public byte[] Buffer { get; }

            public long Length => Buffer.LongLength;

            public FileInfo(IFile decoder, byte[] buffer)
            {
                _decoder = decoder;
                Buffer = buffer;
            }
        }

        private static IDictionary<string, Type> _registeredFileDecoders;
        private readonly DRSConfig _config;

        static FileDecoder()
        {
            RegisterFileDecoders();
        }

        private static void RegisterFileDecoders()
        {
            _registeredFileDecoders = new Dictionary<string, Type>();

            Action<string, Type> register =
                (ext, type) => _registeredFileDecoders.Add(ext.ToLower(), type);

            Array.ForEach(Image.SupportedFileTypes, ext => register(ext, typeof (Image)));
            Array.ForEach(Pdf.SupportedFileTypes, ext => register(ext, typeof (Pdf)));
            Array.ForEach(Text.SupportedFileTypes, ext => register(ext, typeof (Text)));
            Array.ForEach(Default.SupportedFileTypes, ext => register(ext, typeof (Default)));
        }

        /// <summary>
        ///     Returns a file parser for the file type passed in.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws if filename == null</exception>
        private IFile Get(string filename, byte[] buffer)
        {
            var fileExtension = Path.GetExtension(filename)?.ToLower();

            if (fileExtension == null)
            {
                throw new ArgumentException(nameof(fileExtension));
            }

            var parserType = _registeredFileDecoders.ContainsKey(fileExtension)
                ? _registeredFileDecoders[fileExtension]
                : _registeredFileDecoders[".*"];

            return (IFile) Activator.CreateInstance(parserType, buffer, _config);
        }

        public FileDecoder(IOptions<DRSConfig> config)
        {
            _config = config.Value;
        }

        public IFileInfo Decode(string filename, byte[] buffer)
        {
            var decoder = Get(filename, buffer);

            return new FileInfo(decoder, buffer);
        }
    }
}