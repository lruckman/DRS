using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("DistributionGroupTypes", Schema = "Lookup")]
    public class DistributionGroupType : LookupTableBase
    {
    }
}
