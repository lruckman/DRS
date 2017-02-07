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
        Task<IEnumerable<int>> GetUserDistributionGroupIdsAsync(PermissionTypes requestedPermission);
    }

    public class DocumentSecurity : IDocumentSecurity
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserContext _userContext;

        /// <summary>
        /// A cache of user library ids for this user, this instance
        /// </summary>
        private int[] _cachedDistributionGroupIds = { };

        public DocumentSecurity(ApplicationDbContext db, IUserContext userContext)
        {
            _db = db;
            _userContext = userContext;
        }

        public async Task<IEnumerable<int>> GetUserDistributionGroupIdsAsync(PermissionTypes requestedPermission)
        {
            if (_cachedDistributionGroupIds.Length > 0)
            {
                return _cachedDistributionGroupIds;
            }

            var userId = _userContext.UserId;

            return _cachedDistributionGroupIds =
                await _db.NamedDistributions
                    .Where(ul => ul.ApplicationUserId == userId
                                    && (ul.Permissions & requestedPermission) != 0)
                    .Select(ul => ul.DistributionGroupId)
                    .ToArrayAsync()
                    .ConfigureAwait(false);
        }

        public async Task<bool> HasDocumentPermissionAsync(int documentId, PermissionTypes requestedPermission)
        {
            var libraryIds = await GetUserDistributionGroupIdsAsync(requestedPermission).ConfigureAwait(false);

            var docLibraryIds = await _db.Documents
                .Where(d => d.Id == documentId)
                .SelectMany(d => d.Distributions.Select(l => l.DistributionGroupId))
                .ToArrayAsync()
                .ConfigureAwait(false);

            return docLibraryIds.Any(outer => libraryIds.Any(inner => inner == outer));
        }
    }
}