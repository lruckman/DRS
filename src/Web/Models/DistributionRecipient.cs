using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("DistributionRecipients")]
    public class DistributionRecipient
    {
        public DistributionGroup DistributionGroup { get; set; }
        public int DistributionGroupId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }

        public PermissionTypes Permissions { get; set; }
    }
}