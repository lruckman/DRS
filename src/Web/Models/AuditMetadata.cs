using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class AuditMetadata
    {
        [Required, MaxLength(450)]
        public string CreatedBy { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}
