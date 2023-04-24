using System.Threading.Tasks;
using Infra.Shared.Enums;

namespace Infra.Shared.SmsProvider.Abstraction
{
    public interface ISmsService
    {
        Task SendMessageAsync(string phoneNumber, string message);
        Task SendMessageAsync(string phoneNumber, string template, params string[] tokens);
    }
}
