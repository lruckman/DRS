using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using SimpleLucene;
using SimpleLucene.Impl;

namespace Web.Engine.Services.Lucene
{
    public interface ISearcher
    {
        IEnumerable<int> Search(string query);
    }

    public class Searcher : ISearcher
    {
        private readonly string _indexPath;

        public Searcher(DRSSettings settings)
        {
            _indexPath = settings.IndexDirectory;
        }

        public IEnumerable<int> Search(string query)
        {
            var indexSearcher = new DirectoryIndexSearcher(new DirectoryInfo(_indexPath));

            using (var searchService = new SearchService(indexSearcher))
            {
                var results = searchService
                    .SearchIndex(new TermQuery(new Term(Constants.LuceneFieldOCR, query)));

                return results.Results
                    .Select(r => r.GetValue<int>(Constants.LuceneFieldId));
            }
        }
    }
}