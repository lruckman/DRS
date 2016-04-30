using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("Files")]
    public class File
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(256)]
        public string Path { get; set; }

        [Required, MaxLength(256)]
        public string ThumbnailPath { get; set; }

        [Required, MaxLength(16)]
        public string Extension { get; set; }

        [Required]
        public long Size { get; set; }

        [Required]
        public int PageCount { get; set; }

        [MaxLength(1024)]
        public string Key { get; set; }

        public int DocumentId { get; set; }
        public Document Document { get; set; }

        public StatusTypes Status { get; set; }

        [Required, MaxLength(450)]
        public string CreatedByUserId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }

        [Required]
        public int VersionNum { get; set; }
    }
}