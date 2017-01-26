using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("Documents")]
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(60)]
        public string Title { get; set; }

        [MaxLength(512)]
        public string Abstract { get; set; }

        [Required, MaxLength(450)]
        public string CreatedByUserId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }
        
        public virtual List<Distribution> Distributions { get; set; } = new List<Distribution>();
        public virtual List<File> Files { get; set; } = new List<File>();

        public virtual DocumentContent Content { get; set; }

        public StatusTypes Status { get; set; }
    }
}