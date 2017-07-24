using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Engine.Services.Lucene.Definitions;
using Web.Models;

namespace Web.Engine.Services.Lucene
{
    public class IndexService : IFileIndexer
    {
        private readonly Config _config;
        private readonly IIndexDefinition<Revision> _definition;

        public IndexService(IOptions<Config> config, IIndexDefinition<Revision> definition)
        {
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        public void Index(Revision revision) => Index(new[] { revision });

        public void Index(IEnumerable<Revision> entities)
        {
            var actions = entities.Select(e => new
            {
                deletes = _definition.GetIndex(e),
                adds = _definition.Convert(e)
            });

            using (var indexWriter = GetIndexWriter(_config.IndexPath))
            {
                indexWriter.DeleteDocuments(actions.Select(a => a.deletes).ToArray());
                indexWriter.AddDocuments(actions.Select(a => a.adds).ToArray());
            }
        }

        public void Remove(Revision revision)
        {
            using (var indexWriter = GetIndexWriter(_config.IndexPath))
            {
                indexWriter.DeleteDocuments(_definition.GetIndex(revision));
            }
        }

        private static IndexWriter GetIndexWriter(string indexPath, bool isCreate = false)
        {
            var dir = FSDirectory.Open(indexPath);
            var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);

            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer)
            {
                OpenMode = isCreate
                    ? OpenMode.CREATE
                    : OpenMode.CREATE_OR_APPEND,
                // Optional: for better indexing performance, if you
                // are indexing many documents, increase the RAM
                // buffer.
                //
                // RAMBufferSizeMB = 256.0
            };

            return new IndexWriter(dir, config);
        }
    }
}
