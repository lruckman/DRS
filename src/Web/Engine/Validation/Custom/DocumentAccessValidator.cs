using FluentValidation;
using FluentValidation.Validators;
using Web.Engine.Helpers;
using Web.Models;

namespace Web.Engine.Validation.Custom
{
    public class DocumentAcessValidator<T> : PropertyValidator
    {
        private readonly IDocumentSecurity _documentSecurity;
        private readonly PermissionTypes _permission;

        public DocumentAcessValidator(IDocumentSecurity documentSecurity, PermissionTypes permission)
            : base("Unauthorized document access.")
        {
            _documentSecurity = documentSecurity;
            _permission = permission;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            //todo: handle collections
            var documentId = context.PropertyValue as int?;

            if (documentId == null)
            {
                return true;
            }

            return _documentSecurity
                    .HasDocumentPermissionAsync(documentId.Value, _permission)
                    .GetAwaiter()
                    .GetResult();
        }
    }

    public static class DocumentAcessValidatorExtensions
    {
        public static IRuleBuilderOptions<T, TElement> HasDocumentPermission<T, TElement>(
            this IRuleBuilder<T, TElement> ruleBuilder, IDocumentSecurity documentSecurity,
            PermissionTypes permission)
        {
            return ruleBuilder.SetValidator(new DocumentAcessValidator<TElement>(documentSecurity, permission));
        }
    }
}