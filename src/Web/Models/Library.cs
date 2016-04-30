using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("Libraries")]
    public class Library
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(56)]
        public string Name { get; set; }

        [Required, MaxLength(450)]
        public string CreatedByUserId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }

        public virtual List<LibraryDocument> Documents { get; set; } = new List<LibraryDocument>();

        public StatusTypes Status { get; set; }
    }
}