using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using Web.Engine.Helpers;
using Web.Models;

namespace Web.Engine.Validation.Custom
{
    public class LibraryAcessValidator<T> : PropertyValidator
    {
        private readonly IDocumentSecurity _documentSecurity;
        private readonly PermissionTypes _permission;

        public LibraryAcessValidator(IDocumentSecurity documentSecurity, PermissionTypes permission)
            : base("Unauthorized library access.")
        {
            _documentSecurity = documentSecurity;
            _permission = permission;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var requested = context.PropertyValue as int[];

            if (requested == null)
            {
                return true;
            }

            var allowed = _documentSecurity.GetUserDistributionGroupIdsAsync(_permission)
                .GetAwaiter()
                .GetResult();

            return !requested.Except(allowed).Any();
        }
    }

    public static class LibraryAcessValidatorExtensions
    {
        /// <summary>
        ///     Ensures the requesting user has access to the requested library. Validation will fail if the user requests a
        ///     library that they do not have the requested permission on.
        /// </summary>
        /// <typeparam name="T">The object being validated</typeparam>
        /// <typeparam name="TElement">The data type of the property being validated</typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="documentSecurity">Document security helper</param>
        /// <param name="permission">The permission to check for</param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TElement> HasLibraryPermission<T, TElement>(
            this IRuleBuilder<T, TElement> ruleBuilder, IDocumentSecurity documentSecurity,
            PermissionTypes permission)
        {
            return ruleBuilder.SetValidator(new LibraryAcessValidator<TElement>(documentSecurity, permission));
        }
    }
}