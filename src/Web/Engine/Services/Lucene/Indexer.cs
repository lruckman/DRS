using System.Collections.Generic;
using System.IO;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using SimpleLucene.Impl;
using Web.Engine.Services.Lucene.Models;

namespace Web.Engine.Services.Lucene
{
    public interface IIndexer
    {
        bool Index(Index.Command command);
        bool Index(IEnumerable<Index.Command> command);

        void Remove(string id);
        void Remove(IEnumerable<string> ids);
    }

    public class Indexer : IIndexer
    {
        private readonly string _indexPath;

        public Indexer(string indexPath)
        {
            _indexPath = indexPath;
        }

        private IndexService GetIndexService()
        {
            var indexWriter = new DirectoryIndexWriter(new DirectoryInfo(_indexPath), true);
            return new IndexService(indexWriter);
        }

        public bool Index(Index.Command command)
        {
            return Index(new[] {command});
        }

        public bool Index(IEnumerable<Index.Command> command)
        {
            using (var indexService = GetIndexService())
            {
                var result = indexService.IndexEntities(command, c =>
                {
                    var doc = new Document();
                    doc.Add(new Field("id", c.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    doc.Add(new Field("_contents_", c.Contents, Field.Store.NO, Field.Index.ANALYZED));

                    return doc;
                });

                return result.Success;
            }
        }

        public void Remove(string id)
        {
            Remove(new[] {id});
        }

        public void Remove(IEnumerable<string> ids)
        {
            using (var indexService = GetIndexService())
            {
                foreach (var id in ids)
                {
                    indexService.Remove(new Term("id",id));
                }
            }
        }
    }
}
