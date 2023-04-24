using System.Collections.Generic;
using Infra.Shared.Ioc;
using Infra.Validation.Models;

namespace Infra.Validation.Interfaces
{
    public interface IValidator : ITransientDependency
    {
        void Validate(
            List<FieldValidation> fieldValidations,
            bool shouldCheckIsRequired = false);
    }
}