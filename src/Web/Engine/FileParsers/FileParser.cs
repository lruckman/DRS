using System;
using System.IO;
using System.Threading.Tasks;

namespace Web.Engine.FileParsers
{
    public abstract class FileParser : IFileParser
    {
        internal readonly byte[] Buffer;

        protected FileParser(byte[] buffer)
        {
            if (buffer == null || buffer.LongLength==0)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            Buffer = buffer;
            FileLength = buffer.LongLength;
        }

        /// <summary>
        ///     Asynchronously gets the text content for the specific file
        /// </summary>
        /// <param name="pageNumber">The pageNumber to pull content from, set to null to return content from all pages.</param>
        /// <returns>A string containing the parsed text content from the specific file.</returns>
        public async Task<string> GetContentAsync(int? pageNumber = null)
        {
            return await Task.Factory.StartNew(() => Content(pageNumber));
        }

        /// <summary>
        ///     Asynchronously gets the number of pages for the specific file.
        /// </summary>
        /// <returns>An int containing the number of pages contained in the specific file.</returns>
        public async Task<int> GetNumberOfPagesAsync()
        {
            return await Task.Factory.StartNew(() => NumberOfPages());
        }

        /// <summary>
        ///     Asynchronously gets a thunbmail for the specific file.
        /// </summary>
        /// <param name="fileName">Full path where the thumbnail should be saved to.</param>
        /// <param name="pageNumber">The pageNumber you want the thumbnail created for, default pageNumber 1</param>
        /// <param name="dpi">Dots per inch for the thumbnail, default 72</param>
        /// <returns>An array of bytes containing the generated thumbnail</returns>
        public async Task SaveThumbnailAsync(Stream outputStream, int pageNumber = 1, int dpi = 72)
        {
           await Task.Factory.StartNew(() => SaveThumbnail(pageNumber, dpi, outputStream));
        }

        /// <summary>
        ///     Gets the size, in bytes, of the specific file.
        /// </summary>
        public long FileLength { get; }

        protected abstract string Content(int? pageNumber);
        protected abstract int NumberOfPages();
        protected abstract void SaveThumbnail(int pageNumber, int dpi, Stream outputStream);

        public void Dispose()
        {
        }
    }
}