namespace Web.Engine.Codecs.Decoders
{
    public interface IFile
    {
        string ExtractContent(int? pageNumber);
        int ExtractPageCount();
        byte[] ExtractThumbnail(int width, int? height, int pageNumber);
    }
}