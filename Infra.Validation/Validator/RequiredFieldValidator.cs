using FluentValidation;
using Infra.Validation.Interfaces;
using Infra.Validation.Models;

namespace Infra.Validation.Validator
{
    public class RequiredFieldValidator : AbstractValidator<object>, IRequiredFieldValidator
    {
        public RequiredFieldValidator()
        {
            RuleFor(x => x).NotEmpty().NotNull();
        }

        public bool Apply(object value, ValidationRule rule)
        {
            var result = new RequiredFieldValidator();
            return result.Validate(value).IsValid;
        }

    }
}