using System.Text.RegularExpressions;

namespace Web.Engine.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        ///     Normalizes line endings to the standard for Windows \r\n
        /// </summary>
        /// <param name="s"></param>
        /// <returns>The normalized string with line endings converted to \r\n</returns>
        public static string NormalizeLineEndings(this string s)
        {
            return s == null 
                ? null 
                : Regex.Replace(s, @"\r\n|\n\r|\n|\r", "\r\n");
        }

        /// <summary>
        ///     Truncates a string if it is longer than the supplied length
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length">The length to truncate the string to</param>
        /// <returns>A truncated string</returns>
        public static string Truncate(this string s, int length)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            return s.Length < length ? s : s.Substring(0, length);
        }
    }
}