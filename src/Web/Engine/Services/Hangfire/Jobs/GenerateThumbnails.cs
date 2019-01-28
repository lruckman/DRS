using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Engine.Services.Hangfire.Jobs
{
    public class GenerateThumbnails
    {
        private readonly ApplicationDbContext _db;
        private readonly IFileStorage _fileStorage;

        public GenerateThumbnails(ApplicationDbContext db, IFileStorage fileStorage)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));

            Run();
        }

        public void Run()
        {
            Task.WaitAll(IndexNewDocuments());
        }

        private async Task IndexNewDocuments()
        {
            foreach (var dataFile in await _db.Revisions
                .Where(r => r.EndDate == null)
                .Where(r => r.Status == (int)StatusTypes.Active)
                .Select(r => r.DataFile)
                .Where(df => df.ThumbnailPath == null)
                .ToArrayAsync()
                .ConfigureAwait(false))
            {
                using (var file = _fileStorage
                    .Open(dataFile.Path, dataFile.Extension, dataFile.Key, dataFile.IV))
                {
                    var sourceStream = file
                        .CreateThumbnail(new Size(600, 600), 1);

                    using (sourceStream)
                    {
                        dataFile.ThumbnailPath = await _fileStorage
                            .Save(sourceStream, dataFile.Key, dataFile.IV)
                            .ConfigureAwait(false);
                    }
                }
                try
                {
                    await _db.SaveChangesAsync()
                        .ConfigureAwait(false);
                }
                catch
                {
                    _fileStorage.TryDelete(dataFile.ThumbnailPath);

                    throw;
                }
            }
        }
    }
}
