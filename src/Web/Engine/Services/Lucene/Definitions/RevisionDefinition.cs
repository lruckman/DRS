using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using Web.Models;

namespace Web.Engine.Services.Lucene.Definitions
{
    public class RevisionDefinition : IIndexDefinition<Revision>
    {
        public const string RevisionIdField = "documentid";
        public const string RevisionTitleField = "title";
        public const string RevisionAbstractField = "abstract";
        public const string RevisionContentField = "content";
        public const string RevisionDistributionGroupField = "distgrp";

        private readonly IFileDecoder _decoder;
        private readonly IFileEncryptor _encryptor;

        public RevisionDefinition(IFileDecoder decoder, IFileEncryptor encryptor)
        {
            _decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
            _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
        }

        public Term GetIndex(Revision entity)
                => new Term(RevisionIdField, entity.DocumentId.ToString());

        public IEnumerable<IIndexableField> Convert(Revision entity)
        {
            yield return
                new Int32Field(
                        Fields.RevisionIdField,
                        entity.DocumentId,
                        Field.Store.YES);

            var fileKey = _encryptor
                .DecryptBase64(entity.AccessKey);

            var fileinfo = _decoder.Decode(
                entity.Extension,
                _encryptor.DecryptFile(entity.Path, fileKey));

            if (!string.IsNullOrWhiteSpace(fileinfo.Content))
            {
                yield return
                    new TextField(
                        Fields.RevisionContentField,
                        fileinfo.Content,
                        Field.Store.NO);
            }

            if (!string.IsNullOrWhiteSpace(entity.Abstract))
            {
                yield return
                    new TextField(
                        Fields.RevisionAbstractField,
                        entity.Abstract,
                        Field.Store.NO);
            }

            if (!string.IsNullOrWhiteSpace(entity.Title))
            {
                yield return
                    new TextField(
                        Fields.RevisionTitleField,
                        entity.Title,
                        Field.Store.NO);
            }

            foreach (var distributionGroup in entity.Document.Distributions)
            {
                yield return
                    new Int32Field(
                        Fields.RevisionDistributionGroupField,
                        distributionGroup.DistributionGroupId,
                        Field.Store.YES);
            }
        }

        public class QueryBuilder : QueryBase
        {
            public static string[] TextFields = new[] {
                RevisionTitleField,
                RevisionAbstractField,
                RevisionContentField
            };

            public string IndexFieldId => RevisionIdField;

            public QueryBuilder WithKeywords(string keywords)
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
                return new MultiFieldQueryParser(
                    LuceneVersion.LUCENE_48,
                    fields,
                    new StandardAnalyzer(LuceneVersion.LUCENE_48));
            }

            private Query CreateMultiFieldQuery(string[] fields, string keywords)
            {
                var parser = GetMultiFieldQueryParser(fields);

                return parser.Parse(keywords);
            }
        }
    }
}
