using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Web.Engine.FileParsers
{
    public abstract class FileParser : IFileParser
    {
        private static IDictionary<string, Type> _registeredParsers;
        internal readonly byte[] Buffer;

        static FileParser()
        {
            RegisterParsers();
        }

        protected FileParser(byte[] buffer)
        {
            if (buffer == null || buffer.LongLength == 0)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            Buffer = buffer;
            FileLength = buffer.LongLength;
        }

        private static void RegisterParsers()
        {
            _registeredParsers = new Dictionary<string, Type>();

            Action<string, Type> register =
                (ext, type) => { _registeredParsers.Add(ext.ToLower(), type); };

            Array.ForEach(Pdf.SupportedFileTypes, ext => register(ext, typeof (Pdf)));
            Array.ForEach(RawText.SupportedFileTypes, ext => register(ext, typeof(RawText)));
            Array.ForEach(NotSupported.SupportedFileTypes, ext => register(ext, typeof(NotSupported)));
        }

        /// <summary>
        ///     Gets the text content for the specific file
        /// </summary>
        /// <param name="pageNumber">The pageNumber to pull content from, set to null to return content from all pages.</param>
        /// <returns>A string containing the parsed text content from the specific file.</returns>
        public string Content(int? pageNumber)
        {
            return ExtractContent(pageNumber);
        }

        /// <summary>
        ///     Asynchronously gets the text content for the specific file
        /// </summary>
        /// <param name="pageNumber">The pageNumber to pull content from, set to null to return content from all pages.</param>
        /// <returns>A string containing the parsed text content from the specific file.</returns>
        public async Task<string> ContentAsync(int? pageNumber = null)
        {
            return await Task.Factory.StartNew(() => Content(pageNumber));
        }

        protected abstract string ExtractContent(int? pageNumber);

        /// <summary>
        ///     Gets the number of pages for the specific file.
        /// </summary>
        /// <returns>An int containing the number of pages contained in the specific file.</returns>
        public int NumberOfPages()
        {
            return ExtractNumberOfPages();
        }

        /// <summary>
        ///     Asynchronously gets the number of pages for the specific file.
        /// </summary>
        /// <returns>An int containing the number of pages contained in the specific file.</returns>
        public async Task<int> NumberOfPagesAsync()
        {
            return await Task.Factory.StartNew(ExtractNumberOfPages);
        }

        protected abstract int ExtractNumberOfPages();

        /// <summary>
        ///     Gets a thunbmail for the specific file.
        /// </summary>
        /// <param name="outputStream">Stream where the generated thumbnail will be saved to</param>
        /// <param name="pageNumber">The pageNumber you want the thumbnail created for, default pageNumber 1</param>
        /// <param name="dpi">Dots per inch for the thumbnail, default 72</param>
        /// <returns>An array of bytes containing the generated thumbnail</returns>
        public void SaveThumbnail(Stream outputStream, int pageNumber, int dpi)
        {
            if (outputStream == null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            ExtractThumbnail(outputStream, pageNumber, dpi);
        }

        /// <summary>
        ///     Asynchronously gets a thunbmail for the specific file.
        /// </summary>
        /// <param name="outputStream">Stream where the generated thumbnail will be saved to</param>
        /// <param name="pageNumber">The pageNumber you want the thumbnail created for, default pageNumber 1</param>
        /// <param name="dpi">Dots per inch for the thumbnail, default 72</param>
        /// <returns>An array of bytes containing the generated thumbnail</returns>
        public async Task SaveThumbnailAsync(Stream outputStream, int pageNumber = 1, int dpi = 72)
        {
            if (outputStream == null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            await Task.Factory.StartNew(() => SaveThumbnail(outputStream, pageNumber, dpi));
        }

        protected abstract void ExtractThumbnail(Stream outputStream, int pageNumber, int dpi);

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
        public static IFileParser Get(string fileExtension, byte[] buffer)
        {
            if (fileExtension == null)
            {
                throw new ArgumentException(nameof(fileExtension));
            }

            fileExtension = fileExtension.ToLower();

            var parserType = _registeredParsers.ContainsKey(fileExtension)
                ? _registeredParsers[fileExtension]
                : _registeredParsers[".*"];

            return (IFileParser) Activator.CreateInstance(parserType, buffer);
        }
    }
}