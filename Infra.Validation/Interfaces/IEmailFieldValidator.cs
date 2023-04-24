using Infra.Shared.Ioc;

namespace Infra.Validation.Interfaces
{
    public interface IEmailFieldValidator: IValidateFieldValidator, ITransientDependency
    {
    }
}