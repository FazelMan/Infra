using FluentValidation;
using Infra.Validation.Interfaces;
using Infra.Validation.Models;

namespace Infra.Validation.Validator
{
    public class DateTimeFieldValidator : AbstractValidator<object>, IDateTimeFieldValidator
    {
        public DateTimeFieldValidator()
        {
        }

        public bool Apply(object value, ValidationRule rule)
        {
            var result = new DateTimeFieldValidator();
            return true;
        }
    }
}