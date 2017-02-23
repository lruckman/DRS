using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Microsoft.Extensions.Options;
using SimpleLucene;
using SimpleLucene.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Web.Models;

namespace Web.Engine.Services
{
    public interface IFileIndexer
    {
        IndexResult Index(IEnumerable<Revision> revisions);
        IndexResult Index(Revision revision);
        IEnumerable<int> Search(string keywords);
    }

    //todo: clean up
    public class FileIndexer : IFileIndexer
    {
        private const string RevisionIdField = "documentid";
        private const string RevisionTitleField = "title";
        private const string RevisionAbstractField = "abstract";
        private const string RevisionContentField = "content";
        private const string RevisionDistributionGroupField = "distgrp";

        private readonly Config _config;
        private readonly IFileDecoder _decoder;
        private readonly IIndexDefinition<Revision> _indexDefinition;
        private readonly IFileEncryptor _encryptor;

        public FileIndexer(IOptions<Config> config, IFileDecoder decoder, IFileEncryptor encryptor)
        {
            _config = config.Value ?? throw new ArgumentNullException(nameof(config)); //bug
            _decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
            _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
            _indexDefinition = new RevisionIndexDefinition(_decoder, _encryptor);
        }

        public IndexResult Index(IEnumerable<Revision> revisions)
        {
            using (var indexService = GetIndexService())
            {
                return indexService.IndexEntities(revisions, _indexDefinition);
            }
        }

        public IndexResult Index(Revision revision)
            => Index(new[] { revision });

        public IEnumerable<int> Search(string keywords)
        {
            using (var searchService = GetIndexSearcher())
            {
                var query = new RevisionQuery().WithKeywords(keywords);
                var result = searchService.SearchIndex(query.Query);


                return result.Results.Select(d => int.Parse(d.GetValue(RevisionIdField)));
            }
        }

        private IIndexService GetIndexService()
            => new IndexService(new DirectoryIndexWriter(new DirectoryInfo(_config.IndexPath), true));

        private ISearchService GetIndexSearcher()
            => new SearchService(new DirectoryIndexSearcher(new DirectoryInfo(_config.IndexPath), true));

        public class Config
        {
            public string IndexPath { get; set; }
        }

        public class RevisionIndexDefinition : IIndexDefinition<Revision>
        {
            private readonly IFileDecoder _decoder;
            private readonly IFileEncryptor _encryptor;

            public RevisionIndexDefinition(IFileDecoder decoder, IFileEncryptor encryptor)
            {
                _decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
                _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
            }

            public Lucene.Net.Documents.Document Convert(Revision entity)
            {
                var document = new Lucene.Net.Documents.Document();

                document.Add(new Field(RevisionIdField, entity.DocumentId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

                var fileKey = _encryptor
                    .DecryptBase64(entity.AccessKey);

                var fileinfo = _decoder.Decode(entity.Path, _encryptor.DecryptFile(entity.Path, fileKey));

                if (!string.IsNullOrWhiteSpace(fileinfo.Content))
                {
                    document.Add(new Field(RevisionContentField, fileinfo.Content, Field.Store.NO, Field.Index.ANALYZED));
                }

                if (!string.IsNullOrWhiteSpace(entity.Abstract))
                {
                    document.Add(new Field(RevisionAbstractField, entity.Abstract, Field.Store.NO, Field.Index.ANALYZED));
                }

                if (!string.IsNullOrWhiteSpace(entity.Title))
                {
                    document.Add(new Field(RevisionTitleField, entity.Title, Field.Store.NO, Field.Index.ANALYZED));
                }

                foreach (var distributionGroup in entity.Document.Distributions)
                {
                    document.Add(new Field(RevisionDistributionGroupField, distributionGroup.DistributionGroupId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                }

                return document;
            }

            public Term GetIndex(Revision entity)
                => new Term(RevisionIdField, entity.DocumentId.ToString());
        }

        public class RevisionQuery : QueryBase
        {
            public static string[] TextFields = new[] { RevisionTitleField, RevisionAbstractField, RevisionContentField };

            public RevisionQuery WithKeywords(string keywords)
            {
                if (string.IsNullOrWhiteSpace(keywords))
                {
                    return this;
                }

                AddQuery(CreateMultiFieldQuery(TextFields, keywords));

                return this;
            }

            private MultiFieldQueryParser GetMultiFieldQueryParser(string[] fields)
            {
                return new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_29, fields, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29));
            }

            private Query CreateMultiFieldQuery(string[] fields, string keywords)
            {
                var parser = GetMultiFieldQueryParser(fields);

                return parser.Parse(keywords);
            }
        }
    }
}