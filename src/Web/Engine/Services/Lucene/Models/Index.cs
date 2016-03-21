using Lucene.Net.Documents;
using Lucene.Net.Index;
using SimpleLucene;

namespace Web.Engine.Services.Lucene.Models
{
    public class Index
    {
        public class Command
        {
            public int Id { get; set; }
            public string Abstract { get; set; }
            public string Title { get; set; }
            public string Contents { get; set; }
        }

        public class CommandDefinition : IIndexDefinition<Command>
        {
            public Document Convert(Command entity)
            {
                var doc = new Document();

                doc.Add(new Field(Constants.LuceneFieldId, entity.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field(Constants.LuceneFieldOCR, entity.Contents, Field.Store.NO, Field.Index.ANALYZED));
                doc.Add(new Field(Constants.LuceneFieldTitle, entity.Title, Field.Store.NO, Field.Index.ANALYZED));
                doc.Add(new Field(Constants.LuceneFieldAbstract, entity.Abstract, Field.Store.NO, Field.Index.ANALYZED));

                return doc;
            }

            public Term GetIndex(Command entity)
            {
                return new Term(Constants.LuceneFieldId, entity.Id.ToString());
            }
        }
    }
}