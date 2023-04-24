using System.Threading.Tasks;
using Infra.Shared.Ioc;

namespace Infra.SmsProvider.Kavenegar.Services.Interfaces;

public interface IKavenegarSmsService : ITransientDependency
{
    Task SendMessage(string phoneNumber, string template, params string[] tokens);
}