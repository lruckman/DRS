using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Web.Engine.Services
{
    public interface IFileEncryptor
    {
        Stream Encrypt(Stream sourceStream, byte[] key, byte[] iv);
        string Encrypt(byte[] valueToEncrypt);

        (byte[] Key, byte[] IV) GenerateKeyAndIv();
        
        byte[] Decrypt(byte[] buffer, byte[] key = null);
        Stream Decrypt(Stream sourceStream, byte[] key, byte[] iv);

        byte[] DecryptBase64(string s, byte[] key = null);
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

        public (byte[] Key, byte[] IV) GenerateKeyAndIv()
        {
            using (var rm = new RijndaelManaged())
            {
                rm.GenerateKey();
                rm.GenerateIV();

                return (rm.Key, rm.IV);
            }
        }

        public string Encrypt(byte[] valueToEncrypt)
            => ProtectedData.Protect(valueToEncrypt, null, DataProtectionScope).ToBase64String();

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
       
        public byte[] Decrypt(byte[] buffer, byte[] key = null)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return ProtectedData.Unprotect(buffer, key, DataProtectionScope);
        }

        public byte[] DecryptBase64(string s, byte[] key = null)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return Decrypt(Convert.FromBase64String(s), key);
        }

        public Stream Decrypt(Stream sourceStream, byte[] key, byte[] iv)
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

            var decryptor = rijAlg.CreateDecryptor();

            return new CryptoStream(sourceStream, decryptor, CryptoStreamMode.Read);
        }
    }
}