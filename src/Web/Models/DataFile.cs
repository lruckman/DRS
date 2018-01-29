using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("DataFiles")]
    public class DataFile : AuditMetadata
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(256)]
        public string Path { get; set; }

        [MaxLength(256)]
        public string ThumbnailPath { get; set; }

        [Required, MaxLength(16)]
        public string Extension { get; set; }

        [Required]
        public long Size { get; set; }

        [Required]
        public int PageCount { get; set; }

        [MaxLength(1024)]
        public string Key { get; set; }

        [MaxLength(1024)]
        public string IV { get; set; }
    }
}