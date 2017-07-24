using System.Collections.Generic;
using Web.Models;

namespace Web.Engine.Services
{
    public interface IFileIndexer
    {
        void Index(IEnumerable<Revision> entities);
        void Index(Revision revision);
        void Remove(Revision revision);
    }
}
