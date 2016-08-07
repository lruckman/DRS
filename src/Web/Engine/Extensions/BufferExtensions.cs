using System;

namespace Web.Engine.Extensions
{
    public static class BufferExtensions
    {
        /// <summary>
        /// Returns a base64 encoded string representing the provided byte[]
        /// </summary>
        public static string ToBase64String(this byte[] buffer)
        {
            return Convert.ToBase64String(buffer);
        }
    }
}