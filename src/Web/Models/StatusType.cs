using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("StatusTypes", Schema = "Lookup")]
    public class StatusType : LookupTableBase
    {
    }
}