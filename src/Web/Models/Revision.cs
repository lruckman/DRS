using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("Revisions")]
    public abstract class Revision : AuditMetadata
    {
        public int DocumentId { get; set; }
        public virtual Document Document { get; set; }

        public int DataFileId { get; set; }
        public virtual DataFile DataFile { get; set; }

        public int VersionNum { get; set; }

        public int Status { get; set; } //todo: change back to enum once fixed. https://github.com/aspnet/EntityFramework/issues/5529

        public DateTimeOffset? EndDate { get; set; }

        [Required, MaxLength(60)]
        public string Title { get; set; }

        [MaxLength(512)]
        public string Abstract { get; set; }

        public DateTimeOffset? IndexedOn { get; set; }
    }
}