using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("Files")]
    public class File : AuditMetadata
    {
        public int DocumentId { get; set; }
        public virtual Document Document { get; set; }

        public int VersionNum { get; set; }

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
        public string AccessKey { get; set; }

        public StatusTypes Status { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public virtual List<Metadata> Metadata { get; set; } = new List<Metadata>();
    }
}