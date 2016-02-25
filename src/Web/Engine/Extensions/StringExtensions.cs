namespace Web.Engine.Extensions
{
    public static class StringExtensions
    {
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