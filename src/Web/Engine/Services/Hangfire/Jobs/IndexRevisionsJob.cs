using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Web.Models;

namespace Web.Engine.Services.Hangfire.Jobs
{
    public class IndexRevisionsJob
    {
        private readonly IFileIndexer _fileIndexer;
        private readonly ApplicationDbContext _db;

        public IndexRevisionsJob(ApplicationDbContext db, IFileIndexer fileIndexer)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _fileIndexer = fileIndexer ?? throw new ArgumentNullException(nameof(fileIndexer));

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
