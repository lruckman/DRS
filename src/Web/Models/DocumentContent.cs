using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("DocumentContents")]
    public class DocumentContent
    {
        [Key]
        public int Id { get; set; }

        public int DocumentId { get; set; }
        public Document Document { get; set; }

        [MaxLength]
        public string Content { get; set; }
    }
}