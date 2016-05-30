using FluentValidation;
using FluentValidation.Validators;
using Web.Engine.Helpers;
using Web.Models;

namespace Web.Engine.Validation.Custom
{
    public class DocumentFileAccessValidator<T> : PropertyValidator
    {
        private readonly IDocumentSecurity _documentSecurity;
        private readonly PermissionTypes _permission;

        public DocumentFileAccessValidator(IDocumentSecurity documentSecurity, PermissionTypes permission)
            : base("Unauthorized file access.")
        {
            _documentSecurity = documentSecurity;
            _permission = permission;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var fileId = context.PropertyValue as int?;

            return
                _documentSecurity.HasFilePermissionAsync(fileId.Value, _permission)
                    .GetAwaiter()
                    .GetResult();
        }
    }

    public static class HasFilePermissionExtensions
    {
        public static IRuleBuilderOptions<T, TElement> HasDocumentFileAccess<T, TElement>(
            this IRuleBuilder<T, TElement> ruleBuilder, IDocumentSecurity documentSecurity,
            PermissionTypes permission)
        {
            return ruleBuilder.SetValidator(new DocumentFileAccessValidator<TElement>(documentSecurity, permission));
        }
    }
}