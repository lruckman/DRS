using System;
using Web.Models;

namespace Web.Engine.Exceptions
{
    public class UnauthorizedException : UnauthorizedAccessException
    {
        public PermissionTypes RequiredPermission { get; }

        public UnauthorizedException(string message, PermissionTypes requiredPermission) : base(message)
        {
            RequiredPermission = requiredPermission;
        }
    }
}