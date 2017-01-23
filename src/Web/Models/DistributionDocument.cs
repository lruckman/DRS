using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("DistributionDocuments")]
    public class DistributionDocument
    {
        public int DistributionGroupId { get; set; }
        public int DocumentId { get; set; }
        public DistributionGroup DistributionGroup { get; set; }
        public Document Document { get; set; }
    }
}