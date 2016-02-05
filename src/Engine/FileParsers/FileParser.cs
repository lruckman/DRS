using System;
using System.Threading.Tasks;

namespace Engine.FileParsers
{
    public abstract class FileParser : IFileParser
    {
        private byte[] _buffer;

        protected FileParser(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            _buffer = buffer;
        }

        /// <summary>
        ///     Gets the text content for the specific file
        /// </summary>
        /// <param name="pageNumber">The pageNumber to pull content from, set to null to return content from all pages.</param>
        /// <returns>A string containing the parsed text content from the specific file.</returns>
        public async Task<string> Content(int? pageNumber = null)
        {
            return await Task.Factory.StartNew(() => Content(_buffer, pageNumber));
        }

        /// <summary>
        ///     Gets the number of pages for the specific file.
        /// </summary>
        /// <returns>An int containing the number of pages contained in the specific file.</returns>
        public async Task<int> NumberOfPages()
        {
            return await Task.Factory.StartNew(() => NumberOfPages(_buffer));
        }

        /// <summary>
        ///     Gets a thunbmail for the specific file.
        /// </summary>
        /// <param name="fileName">Full path where the thumbnail should be saved to.</param>
        /// <param name="pageNumber">The pageNumber you want the thumbnail created for, default pageNumber 1</param>
        /// <param name="dpi">Dots per inch for the thumbnail, default 72</param>
        /// <returns>An array of bytes containing the generated thumbnail</returns>
        public async Task<string> Thumbnail(string fileName, int pageNumber = 1, int dpi = 72)
        {
            return await Task.Factory.StartNew(() => Thumbnail(_buffer, pageNumber, dpi, fileName));
        }

        /// <summary>
        ///     Gets the size, in bytes, of the specific file.
        /// </summary>
        public long FileLength => _buffer.Length;

        protected abstract string Content(byte[] buffer, int? pageNumber);
        protected abstract int NumberOfPages(byte[] buffer);
        protected abstract string Thumbnail(byte[] buffer, int pageNumber, int dpi, string fileName);

        public void Dispose()
        {
            _buffer = null;
        }
    }
}