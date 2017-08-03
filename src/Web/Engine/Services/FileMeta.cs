using System;

namespace Web.Engine.Services
{
    public class FileMeta
    {
        public FileMeta(byte[] fileContents, string contentType)
        {
            FileContents = fileContents ?? throw new ArgumentNullException(nameof(fileContents));
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
        }

        public byte[] FileContents { get; set; }
        public string ContentType { get; set; }
    }
}
