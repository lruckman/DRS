using System;

namespace Web.Models
{
    [Flags]
    public enum PermissionTypes
    {
        Read = 1,
        Write = 2,
        Modify = 4,
        Delete = 8,
        Share = 16,
        Archive = 32,
        Full = Read | Write | Modify | Delete | Share | Archive
    }
}