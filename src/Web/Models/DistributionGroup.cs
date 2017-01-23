using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("DistributionGroups")]
    public class DistributionGroup
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(56)]
        public string Name { get; set; }

        [Required, MaxLength(450)]
        public string CreatedByUserId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }

        public StatusTypes Status { get; set; }

        public virtual List<DistributionDocument> DistributionDocuments { get; set; } = new List<DistributionDocument>();
        public virtual List<DistributionRecipient> Recipients { get; set; } = new List<DistributionRecipient>();
    }
}
