using Lucene.Net.Search;

namespace Web.Engine.Services.Lucene
{
    public class QueryBase
    {
        private readonly BooleanQuery baseQuery = new BooleanQuery();

        protected QueryBase() { }

        protected QueryBase(Query query)
        {
            AddQuery(query);
        }

        public Query Query
        {
            get { return baseQuery; }
        }

        protected void AddQuery(Query query)
        {
            AddQuery(query, Occur.MUST);
        }

        protected void AddQuery(Query query, Occur occur)
        {
            if (query != null)
            {
                baseQuery.Add(query, occur);
            }
        }
    }
}
