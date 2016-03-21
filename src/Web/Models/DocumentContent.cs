using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
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