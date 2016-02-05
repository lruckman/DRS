using System;
using System.IO;
using System.Security.Cryptography;

namespace Engine.Extensions
{
    public static class Buffers
    {
        public static void Encrypt(this byte[] buffer, MemoryProtectionScope scope)
        {
            if (buffer.Length <= 0)
            {
                throw new ArgumentException(nameof(buffer));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            // Encrypt the data in memory. The result is stored in the same same array as the original data.

            ProtectedMemory.Protect(buffer, scope);
        }

        public static void Decrypt(this byte[] buffer, MemoryProtectionScope scope)
        {
            if (buffer.Length <= 0)
            {
                throw new ArgumentException(nameof(buffer));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            // Decrypt the data in memory. The result is stored in the same same array as the original data.

            ProtectedMemory.Unprotect(buffer, scope);
        }

        public static int EncryptToStream(this byte[] buffer, byte[] entropy, DataProtectionScope scope, Stream stream)
        {
            if (buffer.Length <= 0)
            {
                throw new ArgumentException(nameof(buffer));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (entropy.Length <= 0)
            {
                throw new ArgumentException(nameof(entropy));
            }

            if (entropy == null)
            {
                throw new ArgumentNullException(nameof(entropy));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var length = 0;

            // Encrypt the data in memory. The result is stored in the same same array as the original data.

            var encrptedData = ProtectedData.Protect(buffer, entropy, scope);

            if (!stream.CanWrite)
            {
                return length;
            }

            // Write the encrypted data to a stream.

            stream.Write(encrptedData, 0, encrptedData.Length);

            length = encrptedData.Length;

            // Return the length that was written to the stream. 

            return length;

        }

        public static byte[] DecryptToBytes(this Stream stream, byte[] entropy, DataProtectionScope scope, int length)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (length <= 0)
            {
                throw new ArgumentException(nameof(length));
            }

            if (entropy == null)
            {
                throw new ArgumentNullException(nameof(entropy));
            }

            if (entropy.Length <= 0)
            {
                throw new ArgumentException(nameof(entropy));
            }

            var inBuffer = new byte[length];
            byte[] outBuffer;

            // Read the encrypted data from a stream.

            if (stream.CanRead)
            {
                stream.Read(inBuffer, 0, length);

                outBuffer = ProtectedData.Unprotect(inBuffer, entropy, scope);
            }
            else
            {
                throw new IOException("Could not read the stream.");
            }

            // Return the length that was written to the stream. 

            return outBuffer;

        }
    }
}