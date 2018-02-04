using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Web.Models;

namespace Web.Engine.Services.Hangfire.Jobs
{
    public class IndexRevisions
    {
        private readonly IFileIndexer _fileIndexer;
        private readonly ApplicationDbContext _db;

        public IndexRevisions(ApplicationDbContext db, IFileIndexer fileIndexer)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _fileIndexer = fileIndexer ?? throw new ArgumentNullException(nameof(fileIndexer));

            Run();
        }

        public void Run()
        {
            IndexNewDocuments();
        }

        private void IndexNewDocuments()
        {
            foreach (var revision in _db.Revisions
                .Include(r => r.DataFile)
                .Include(r => r.Document)
                .Include(r => r.Document.Distributions)
                .Where(r => r.EndDate == null)
                .Where(r => r.IndexedOn == null)
                .Where(r => r.Status == (int)StatusTypes.Active || r.Status == (int)StatusTypes.Deleted)
                .OrderBy(r => r.CreatedOn) // need to keep this in chronological order
                .ToArray())
            {
                if (revision.Status == (int)StatusTypes.Deleted)
                {
                    _fileIndexer.Remove(revision);
                }
                else if (revision.Status == (int)StatusTypes.Active)
                {
                    _fileIndexer.Index(revision);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(revision.Status), revision.Status, "Status value not expected.");
                }

                revision.IndexedOn = DateTimeOffset.Now;

                _db.SaveChanges();
            }
        }

        private void RemoveDeletedDocuments()
        {
            //foreach (var revision in _db.DeletedRevisions
            //    .Include(dr => dr.Document)
            //    .Include(dr => dr.Document.Distributions)
            //    .Where(dr => dr.EndDate == null && dr.IndexedOn == null)
            //    .ToArray())
            //{
            //    _fileIndexer.Index(revision);

            //    revision.IndexedOn = DateTimeOffset.Now;

            //    _db.SaveChanges();
            //}

            throw new NotImplementedException();
        }
    }
}
