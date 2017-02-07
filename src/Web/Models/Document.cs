using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("Documents")]
    public class Document : AuditMetadata
    {
        [Key]
        public int Id { get; set; }

        public virtual List<Distribution> Distributions { get; set; } = new List<Distribution>();
        public virtual List<File> Files { get; set; } = new List<File>();

        public virtual DocumentContent Content { get; set; } //todo: remove when lucene
    }
}