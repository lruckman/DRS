using System;
using System.IO;

namespace Web.Engine.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            stream.Position = 0;
            var buffer = new byte[stream.Length];
            for (var totalBytesCopied = 0; totalBytesCopied < stream.Length;)
            {
                totalBytesCopied += stream.Read(buffer, totalBytesCopied,
                    Convert.ToInt32(stream.Length) - totalBytesCopied);
            }
            return buffer;
        }
    }
}