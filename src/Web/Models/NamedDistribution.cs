using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("NamedDistributions")]
    public class NamedDistribution
    {
        public int DistributionGroupId { get; set; }
        public DistributionGroup DistributionGroup { get; set; }
        
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public PermissionTypes Permissions { get; set; }
    }
}
