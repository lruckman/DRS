using Web.Engine.Codecs.Decoders;

namespace Web.Engine.Services
{
    public interface IFileDecoder
    {
        IDecoder Get(string filename);
    }
}
