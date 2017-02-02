using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Web.Engine.Extensions;

namespace Web.Engine.Services
{
    public interface IFileEncryptor
    {
        byte[] Encrypt(byte[] buffer, byte[] key);
        byte[] Decrypt(Stream stream, byte[] key = null);
        byte[] Decrypt(byte[] buffer, byte[] key = null);
        byte[] Decrypt(string s, byte[] key = null);
        byte[] DecryptBase64(string s, byte[] key = null);
        byte[] DecryptFile(string path, byte[] key = null);
    }

    public class FileEncryptor : IFileEncryptor
    {
        private const DataProtectionScope DataProtectionScope =
            System.Security.Cryptography.DataProtectionScope.LocalMachine;

        /// <summary>
        ///     Encrypts the provided byte[]. The byte[] passed in will have its reference updated.
        /// </summary>
        /// <returns>Returns the encrypted byte[]</returns>
        /// <exception cref="ArgumentNullException">Throws if buffer == null</exception>
        public byte[] Encrypt(byte[] buffer, byte[] key)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return buffer = ProtectedData.Protect(buffer, key, DataProtectionScope);
        }

        public byte[] Decrypt(Stream stream, byte[] key = null)
        {
            return Decrypt(stream.ToByteArray(), key);
        }

        public byte[] Decrypt(byte[] buffer, byte[] key = null)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return ProtectedData.Unprotect(buffer, key, DataProtectionScope);
        }

        public byte[] Decrypt(string s, byte[] key = null)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return Decrypt(Encoding.UTF8.GetBytes(s), key);
        }

        public byte[] DecryptBase64(string s, byte[] key = null)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return Decrypt(Convert.FromBase64String(s), key);
        }

        public byte[] DecryptFile(string path, byte[] key = null)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException("Path does not exist.", nameof(path));
            }

            return Decrypt(File.ReadAllBytes(path), key);
        }
    }
}