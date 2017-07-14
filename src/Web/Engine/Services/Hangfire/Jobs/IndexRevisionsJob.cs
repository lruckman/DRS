using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using Web.Models;

namespace Web.Engine.Services.Hangfire.Jobs
{
    public class IndexRevisionsJob
    {
        private readonly IFileIndexer _fileIndexer;
        private readonly ApplicationDbContext _db;

        //bug: needs fixing when #8 is resolved
        //everythings hardcoded for now :(
        public IndexRevisionsJob(/*ApplicationDbContext db, IFileIndexer fileIndexer*/)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=.\\;Database=DRS;Trusted_Connection=True;MultipleActiveResultSets=true");

            _db = new ApplicationDbContext(optionsBuilder.Options);

            var decoder = new FileDecoder(Options.Create(new DRSConfig
            {
                BasePath = "C:\\Development\\DRS\\src\\Web\\wwwroot",
                DocumentPath = "C:\\Development\\DRS\\src\\Web\\wwwroot\\App_Data\\documents",
                TessDataPath = "C:\\Development\\DRS\\src\\Web\\bin\\Debug\\net451\\win7-x86\\tessdata"
            }));

            var encryptor = new FileEncryptor();

            _fileIndexer = new FileIndexer(Options.Create(new FileIndexer.Config
            {
                IndexPath = "C:\\Development\\DRS\\src\\Web\\wwwroot\\App_Data\\lucene"
            }), decoder, encryptor);

            Run();
        }

        public void Run()
        {
            foreach (var revision in _db.PublishedRevisions
                .Include(pr => pr.Document)
                .Include(pr => pr.Document.Distributions)
                .Where(pr => pr.EndDate == null && pr.IndexedOn == null)
                .ToArray())
            {
                _fileIndexer.Index(revision);

                revision.IndexedOn = DateTimeOffset.Now;

                _db.SaveChanges();
            }
        }
    }
}
