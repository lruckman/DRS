using System.Collections.Generic;

namespace Web.Engine.Services
{
    public interface IFileSearcher
    {
        IEnumerable<int> Search(string keywords);
    }
}
