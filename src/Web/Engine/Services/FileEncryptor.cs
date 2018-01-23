using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Web.Engine.Extensions;

namespace Web.Engine.Services
{
    public interface IFileEncryptor
    {
        Stream Encrypt(Stream sourceStream, byte[] key, byte[] iv);
        byte[] Encrypt(string valueToEncrypt);

        (byte[] Key, byte[] IV) GenerateKeyAndIv();

        byte[] Decrypt(Stream stream, byte[] key = null);
        byte[] Decrypt(byte[] buffer, byte[] key = null);
        byte[] Decrypt(string s, byte[] key = null);
        byte[] DecryptBase64(string s, byte[] key = null);
        byte[] DecryptFile(string path, byte[] key = null);
    }

    public static class FileEncryptorExtensions
    {
        public static string ToBase64String(this byte[] buffer)
            => Convert.ToBase64String(buffer);

        public static byte[] ToByteArray(this string input)
            => Encoding.UTF8.GetBytes(input);
    }

    public class FileEncryptor : IFileEncryptor
    {
        private const DataProtectionScope DataProtectionScope =
            System.Security.Cryptography.DataProtectionScope.LocalMachine;
        
        public string GenerateKey()
            => Encrypt(Guid.NewGuid().ToString("N")).ToBase64String();

        public (byte[] Key, byte[] IV) GenerateKeyAndIv()
        {
            using (var rm = new RijndaelManaged())
            {
                rm.GenerateKey();
                rm.GenerateIV();

                return (rm.Key, rm.IV);
            }
        }

        public byte[] Encrypt(string valueToEncrypt)
        {
            if (string.IsNullOrWhiteSpace(valueToEncrypt))
            {
                throw new ArgumentNullException(nameof(valueToEncrypt));
            }

            var userData = valueToEncrypt.ToByteArray();

            return ProtectedData.Protect(userData, null, DataProtectionScope);
        }

        public Stream Encrypt(Stream sourceStream, byte[] key, byte[] iv)
        {
            if (sourceStream == null)
            {
                throw new ArgumentNullException(nameof(sourceStream));
            }

            if (!sourceStream.CanRead)
            {
                throw new ArgumentException("Stream cannot be read.", nameof(sourceStream));
            }

            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(iv));
            }

            var rijAlg = new RijndaelManaged
            {
                Key = key,
                IV = iv
            };

            var encryptor = rijAlg.CreateEncryptor();

            return new CryptoStream(sourceStream, encryptor, CryptoStreamMode.Read);
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