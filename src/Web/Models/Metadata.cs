using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("Metadata")]
    public class Metadata : AuditMetadata
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(60)]
        public string Title { get; set; }

        [MaxLength(512)]
        public string Abstract { get; set; }

        public int VersionNum { get; set; }
        public int DocumentId { get; set; }

        public File File { get; set; }

        public DateTimeOffset? EndDate { get; set; }
    }
}
