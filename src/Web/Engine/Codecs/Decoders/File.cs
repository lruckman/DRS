namespace Web.Engine.Codecs.Decoders
{
    public abstract class File : IFile
    {
        internal readonly byte[] Buffer;
        internal readonly DRSConfig Config;

        protected File(byte[] buffer, DRSConfig config)
        {
            Buffer = buffer;
            Config = config;
        }

        public abstract string ExtractContent(int? pageNumber);
        public abstract int ExtractPageCount();
        public abstract byte[] ExtractThumbnail(int width, int? height, int pageNumber);
    }
}