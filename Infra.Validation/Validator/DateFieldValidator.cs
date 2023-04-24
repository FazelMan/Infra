using FluentValidation;
using Infra.Validation.Interfaces;
using Infra.Validation.Models;

namespace Infra.Validation.Validator
{
    public class DateFieldValidator : AbstractValidator<object>, IDateFieldValidator
    {
        public DateFieldValidator()
        {
        }

        public bool Apply(object value, ValidationRule rule)
        {
            var result = new DateFieldValidator();
            return result.Validate(value).IsValid;
        }

    }
}