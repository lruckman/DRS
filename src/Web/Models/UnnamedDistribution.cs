using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("UnnamedDistributions")]
    public class UnnamedDistribution
    {
        [Key]
        public int DistributionGroupId { get; set; }
        public DistributionGroup DistributionGroup { get; set; }

        public Guid AccessKey { get; set; }
        public DateTimeOffset ExpiryDate { get; set; }
        public PermissionTypes Permissions { get; set; }
    }
}
