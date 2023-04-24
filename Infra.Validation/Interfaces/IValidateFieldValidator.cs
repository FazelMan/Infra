using Infra.Validation.Models;

namespace Infra.Validation.Interfaces
{
    public interface IValidateFieldValidator
    {
        bool Apply(object value, ValidationRule rule);
    }
}