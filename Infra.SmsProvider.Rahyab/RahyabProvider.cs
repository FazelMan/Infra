using System.Threading.Tasks;
using Amazon.S3;
using Infra.Shared.Attributes;
using Infra.Shared.Enums;
using Infra.Shared.SmsProvider.Abstraction;
using Infra.SmsProvider.Rahyab.Services.Interfaces;

namespace Infra.SmsProvider.Rahyab;

[Name(Name)]
public class RahyabProvider : ISmsService
{
    private readonly IRahyabSmsService _rahyabSmsService;

    public RahyabProvider(IRahyabSmsService rahyabSmsService)
    {
        _rahyabSmsService = rahyabSmsService;
    }

    public const string Name = nameof(RahyabProvider);

    public Task SendMessageAsync(string phoneNumber, string template, params string[] tokens)
    {
        throw new System.NotImplementedException();
    }

    public async Task SendMessageAsync(string phoneNumber, string message)
    {
        await Task.Run(() => { _rahyabSmsService.SendSingle(phoneNumber, message); });
    }
}