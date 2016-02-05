using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Web.Engine.FileParsers;

namespace Web.Engine.Extensions
{
    public static class BufferExtensions
    {
        /// <summary>
        ///     Asynchronously encrypts and saves the contents of an uploaded file.
        /// </summary>
        /// <param name="buffer">The <see cref="byte[]" />.</param>
        /// <param name="filename">The name of the file to create.</param>
        /// <param name="optionalEntropy">
        ///     An optional additional byte array used to increase the complexity of the encryption, or
        ///     null for no additional complexity.
        /// </param>
        /// <param name="scope"> One of the enumeration values that specifies the scope of encryption. </param>
        /// <param name="cancellationToken"></param>
        public static async Task SaveProtectedAsAsync(this byte[] buffer, string filename,
            byte[] optionalEntropy, DataProtectionScope scope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buffer == null || buffer.LongLength == 0)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException(nameof(filename));
            }

            using (var fileStream = new FileStream(filename, FileMode.Create))
            {
                await buffer
                    .ProtectToStreamAsync(optionalEntropy, scope, fileStream, cancellationToken);
            }
        }

        //public static void Encrypt(this byte[] buffer, MemoryProtectionScope scope)
        //{
        //    if (buffer.Length <= 0)
        //    {
        //        throw new ArgumentException(nameof(buffer));
        //    }

        //    if (buffer == null)
        //    {
        //        throw new ArgumentNullException(nameof(buffer));
        //    }

        //    // Encrypt the data in memory. The result is stored in the same same array as the original data.

        //    ProtectedMemory.Protect(buffer, scope);
        //}

        public static byte[] Unprotect(this byte[] buffer, byte[] optionalEntropy,
            DataProtectionScope scope)
        {
            if (buffer.Length <= 0)
            {
                throw new ArgumentException(nameof(buffer));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            // Unprotect the data

            return ProtectedData.Unprotect(buffer, optionalEntropy, scope);
        }

        /// <summary>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="optionalEntropy">
        ///     An optional additional byte array used to increase the complexity of the encryption, or
        ///     null for no additional complexity.
        /// </param>
        /// <param name="scope"> One of the enumeration values that specifies the scope of encryption. </param>
        /// <param name="stream"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<int> ProtectToStreamAsync(this byte[] buffer, byte[] optionalEntropy,
            DataProtectionScope scope, Stream stream, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buffer.Length <= 0)
            {
                throw new ArgumentException(nameof(buffer));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var length = 0;

            // Encrypt the data in memory. The result is stored in the same same array as the original data.

            var encrptedData = ProtectedData.Protect(buffer, optionalEntropy, scope);

            if (!stream.CanWrite)
            {
                return length;
            }

            // Write the encrypted data to a stream.

            await stream.WriteAsync(encrptedData, 0, encrptedData.Length, cancellationToken);

            length = encrptedData.Length;

            // Return the length that was written to the stream. 

            return length;
        }

        public static byte[] Protect(this byte[] buffer, byte[] optionalEntropy, DataProtectionScope scope)
        {
            return ProtectedData.Protect(buffer, optionalEntropy, scope);
        }

        //public static byte[] DecryptToBytes(this Stream stream, byte[] optionalEntropy, DataProtectionScope scope, int length)
        //{
        //    if (stream == null)
        //    {
        //        throw new ArgumentNullException(nameof(stream));
        //    }

        //    if (length <= 0)
        //    {
        //        throw new ArgumentException(nameof(length));
        //    }

        //    if (optionalEntropy == null)
        //    {
        //        throw new ArgumentNullException(nameof(optionalEntropy));
        //    }

        //    if (optionalEntropy.Length <= 0)
        //    {
        //        throw new ArgumentException(nameof(optionalEntropy));
        //    }

        //    var inBuffer = new byte[length];
        //    byte[] outBuffer;

        //    // Read the encrypted data from a stream.

        //    if (stream.CanRead)
        //    {
        //        stream.Read(inBuffer, 0, length);

        //        outBuffer = ProtectedData.Unprotect(inBuffer, optionalEntropy, scope);
        //    }
        //    else
        //    {
        //        throw new IOException("Could not read the stream.");
        //    }

        //    // Return the length that was written to the stream. 

        //    return outBuffer;

        //}

        public static IFileParser Parse(this byte[] buffer, string fileExtension)
        {
            return new Pdf(buffer);
        }
    }
}