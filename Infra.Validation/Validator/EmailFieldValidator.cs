using FluentValidation;
using Infra.Validation.Interfaces;
using Infra.Validation.Models;

namespace Infra.Validation.Validator
{
    public class EmailFieldValidator : AbstractValidator<string>, IEmailFieldValidator
    {
        public EmailFieldValidator()
        {
            RuleFor(x => x).EmailAddress();
        }

        public bool Apply(object value, ValidationRule rule)
        {
            var result = new EmailFieldValidator();
            return result.Validate(value.ToString()).IsValid;
        }
    }
}