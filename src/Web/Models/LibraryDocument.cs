using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("LibraryDocuments")]
    public class LibraryDocument
    {
        public int LibraryId { get; set; }
        public int DocumentId { get; set; }
        public Library Library { get; set; }
        public Document Document { get; set; }
    }
}