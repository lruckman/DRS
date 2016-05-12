using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("UserDocuments")]
    public class UserDocument
    {
        public Document Document { get; set; }
        public int DocumentId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }

        public PermissionTypes Permissions { get; set; }
    }
}