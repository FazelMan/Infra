using FluentValidation;
using Infra.Validation.Interfaces;
using Infra.Validation.Models;

namespace Infra.Validation.Validator
{
    public class LengthFieldValidator : AbstractValidator<string>, ILengthFieldValidator
    {
        public LengthFieldValidator()
        {
        }

        public bool Apply(object value, ValidationRule rule)
        {
            var result = new DateTimeFieldValidator();
            return result.Validate(value).IsValid;
        }

    }
}