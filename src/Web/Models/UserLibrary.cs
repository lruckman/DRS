using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("UserLibraries")]
    public class UserLibrary
    {
        public Library Library { get; set; }
        public int LibraryId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }

        public PermissionTypes Permissions { get; set; }
    }
}