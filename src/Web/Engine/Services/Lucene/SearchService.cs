using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Engine.Services.Lucene.Definitions;

namespace Web.Engine.Services.Lucene
{
    public class SearchService : IFileSearcher
    {
        private readonly Config _config;
        private readonly IFileDecoder _decoder;
        private readonly IFileEncryptor _encryptor;

        public SearchService(IOptions<Config> config, IFileDecoder decoder, IFileEncryptor encryptor)
        {
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
            _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
        }

        public IEnumerable<int> Search(string keywords)
        {
            var dir = FSDirectory.Open(_config.IndexPath);
            using (var reader = DirectoryReader.Open(dir))
            {
                var searcher = new IndexSearcher(reader);
                var query = new RevisionDefinition.QueryBuilder().WithKeywords(keywords);
                var result = searcher.Search(query.Query, 25); //todo: paging

                var hits = result.ScoreDocs;

                var docs = hits
                    .Select(d => searcher.Doc(d.Doc))
                    .ToArray();

                return docs
                    .Select(d => int.Parse(d.Get(query.IndexFieldId)))
                    .ToArray();
            }
        }
    }
}
