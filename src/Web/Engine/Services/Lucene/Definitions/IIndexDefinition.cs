using Lucene.Net.Index;
using System.Collections.Generic;

namespace Web.Engine.Services.Lucene.Definitions
{
    public interface IIndexDefinition<TEntity>
    {
        IEnumerable<IIndexableField> Convert(TEntity entity);
        Term GetIndex(TEntity entity);
    }
}
