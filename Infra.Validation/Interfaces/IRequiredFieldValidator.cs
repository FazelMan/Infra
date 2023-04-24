using Infra.Shared.Ioc;

namespace Infra.Validation.Interfaces
{
    public interface IRequiredFieldValidator: IValidateFieldValidator, ITransientDependency
    {
    }
}