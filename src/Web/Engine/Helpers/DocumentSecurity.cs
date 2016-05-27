using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Engine.Helpers
{
    public interface IDocumentSecurity
    {
        Task<bool> HasDocumentPermissionAsync(int documentId, PermissionTypes requestedPermission);
        Task<bool> HasFilePermissionAsync(int fileId, PermissionTypes requestedPermission);
        Task<IEnumerable<int>> GetUserLibraryIdsAsync(PermissionTypes requestedPermission);
    }

    public class DocumentSecurity : IDocumentSecurity
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserAccessor _userAccessor;

        /// <summary>
        /// A cache of user library ids for this user, this instance
        /// </summary>
        private int[] _cacheUserLibraryIds = {};

        public DocumentSecurity(ApplicationDbContext db, IUserAccessor userAccessor)
        {
            _db = db;
            _userAccessor = userAccessor;
        }

        public async Task<IEnumerable<int>> GetUserLibraryIdsAsync(PermissionTypes requestedPermission)
        {
            if (_cacheUserLibraryIds.Any())
            {
                return _cacheUserLibraryIds;
            }

            var userId = _userAccessor.UserId;

            return _cacheUserLibraryIds =
                await _db.UserLibraries
                    .Where(ul => ul.ApplicationUserId == userId &&
                                 (ul.Permissions & requestedPermission) != 0)
                    .Select(ul => ul.LibraryId)
                    .ToArrayAsync();
        }

        public async Task<bool> HasDocumentPermissionAsync(int documentId, PermissionTypes requestedPermission)
        {
            var libraryIds = await GetUserLibraryIdsAsync(requestedPermission);

            var docLibraryIds = await _db.Documents
                .Where(d => d.Id == documentId)
                .SelectMany(d => d.Libraries.Select(l=>l.LibraryId))
                .ToArrayAsync();

            return docLibraryIds.Any(outer => libraryIds.Any(inner => inner == outer));
        }

        public async Task<bool> HasFilePermissionAsync(int fileId, PermissionTypes requestedPermission)
        {
            var libraryIds = await GetUserLibraryIdsAsync(requestedPermission);

            var fileIds = await _db.Files
                .Where(f => f.Id == fileId)
                .SelectMany(f => f.Document.Libraries.Select(l=>l.LibraryId))
                .ToArrayAsync();

            return fileIds.Any(outer => libraryIds.Any(inner => inner == outer));
        }
    }
}