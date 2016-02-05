using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Library
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(56)]
        public string Name { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }

        public virtual List<LibraryDocument> Documents { get; set; } = new List<LibraryDocument>();
    }
}