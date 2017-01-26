using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("Distributions")]
    public class Distribution
    {
        public int DistributionGroupId { get; set; }
        public int DocumentId { get; set; }
        public DistributionGroup DistributionGroup { get; set; }
        public Document Document { get; set; }
    }
}