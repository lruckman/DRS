using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(60)]
        public string Title { get; set; }

        [MaxLength(512)]
        public string Abstract { get; set; }

        [Required, MaxLength(256)]
        public string Path { get; set; }

        [Required, MaxLength(256)]
        public string ThumbnailPath { get; set; }

        [Required, MaxLength(16)]
        public string Extension { get; set; }

        [Required]
        public long FileSize { get; set; }

        [Required]
        public int PageCount { get; set; }

        [MaxLength(1024)]
        public string Key { get; set; }

        [Required, MaxLength(450)]
        public string CreatedByUserId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }

        public virtual List<LibraryDocument> Libraries { get; set; } = new List<LibraryDocument>();

        public virtual DocumentContent Content { get; set; }
    }
}