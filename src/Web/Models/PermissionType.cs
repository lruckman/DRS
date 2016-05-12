using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("PermissionTypes", Schema = "Lookup")]
    public class PermissionType : LookupTableBase
    {
    }
}