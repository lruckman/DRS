using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Web.Engine.Codecs.Decoders
{
    public abstract class File : IFile
    {
        private static IDictionary<string, Type> _registeredFileDecoders;
        internal readonly byte[] Buffer;
        internal readonly string BaseDirectory;

        static File()
        {
            RegisterFileDecoders();
        }

        protected File(byte[] buffer, string baseDirectory)
        {
            if (buffer == null || buffer.LongLength == 0)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (!System.IO.File.Exists(baseDirectory))
            {
                throw new ArgumentException("Path does not exist or access is denied.", nameof(baseDirectory));
            }

            BaseDirectory = baseDirectory;
            Buffer = buffer;
            FileLength = buffer.LongLength;
        }

        private static void RegisterFileDecoders()
        {
            _registeredFileDecoders = new Dictionary<string, Type>();

            Action<string, Type> register =
                (ext, type) => { _registeredFileDecoders.Add(ext.ToLower(), type); };

            Array.ForEach(Image.SupportedFileTypes, ext => register(ext, typeof (Image)));
            Array.ForEach(Pdf.SupportedFileTypes, ext => register(ext, typeof (Pdf)));
            Array.ForEach(Text.SupportedFileTypes, ext => register(ext, typeof (Text)));
            Array.ForEach(Default.SupportedFileTypes, ext => register(ext, typeof (Default)));
        }

        /// <summary>
        ///     Asynchronously gets the text content for the specific file
        /// </summary>
        /// <param name="pageNumber">The pageNumber to pull content from, set to null to return content from all pages.</param>
        /// <returns>A string containing the parsed text content from the specific file.</returns>
        public async Task<string> ContentAsync(int? pageNumber = null)
        {
            return await Task.Factory.StartNew(() => ExtractContent(pageNumber));
        }

        protected abstract string ExtractContent(int? pageNumber);

        /// <summary>
        ///     Asynchronously gets the number of pages for the specific file.
        /// </summary>
        /// <returns>An int containing the number of pages contained in the specific file.</returns>
        public async Task<int> PageCountAsync()
        {
            return await Task.Factory.StartNew(ExtractPageCount);
        }

        protected abstract int ExtractPageCount();

        /// <summary>
        ///     Asynchronously gets a thunbmail for the specific file.
        /// </summary>
        /// <param name="outputStream">Stream where the generated thumbnail will be saved to</param>
        /// <param name="pageNumber">The pageNumber you want the thumbnail created for, default pageNumber 1</param>
        /// <param name="width">Width of the generated thumbnail</param>
        /// <param name="height">Height of the generate thumbnail. If null the thumbnail will be scaled to meet just the width.</param>
        /// <returns>An array of bytes containing the generated thumbnail</returns>
        public async Task ThumbnailAsync(Stream outputStream, int width, int? height = null, int pageNumber = 1)
        {
            if (outputStream == null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            await Task.Factory.StartNew(() => ExtractThumbnail(outputStream, width, height, pageNumber));
        }

        protected abstract void ExtractThumbnail(Stream outputStream, int width, int? height, int pageNumber);

        /// <summary>
        ///     Gets the size, in bytes, of the specific file.
        /// </summary>
        public long FileLength { get; }

        /// <summary>
        ///     Returns a file parser for the file type passed in.
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static IFile Get(string fileExtension, byte[] buffer, string baseDirectory)
        {
            if (fileExtension == null)
            {
                throw new ArgumentException(nameof(fileExtension));
            }

            fileExtension = fileExtension.ToLower();

            var parserType = _registeredFileDecoders.ContainsKey(fileExtension)
                ? _registeredFileDecoders[fileExtension]
                : _registeredFileDecoders[".*"];

            return (IFile) Activator.CreateInstance(parserType, buffer, baseDirectory);
        }
    }
}