using System;
using System.Threading.Tasks;
using Amazon.S3;
using Infra.Shared.Attributes;
using Infra.Shared.Enums;
using Infra.Shared.SmsProvider.Abstraction;
using Infra.SmsProvider.Kavenegar.Services.Interfaces;

namespace Infra.SmsProvider.Kavenegar;

[Name(Name)]
public class KavenegarProvider : ISmsService
{
    public const string Name = nameof(KavenegarProvider);
    private readonly IKavenegarSmsService _kavenegarSmsService;

    public KavenegarProvider(IKavenegarSmsService kavenegarSmsService)
    {
        _kavenegarSmsService = kavenegarSmsService;
    }

    public Task SendMessageAsync(string phoneNumber, string template, params string[] tokens)
    {
        return Task.Run(async () => { await _kavenegarSmsService.SendMessage(phoneNumber, template, tokens); });
    }

    public Task SendMessageAsync(string phoneNumber, string message)
    {
        throw new System.NotImplementedException();
    }
}