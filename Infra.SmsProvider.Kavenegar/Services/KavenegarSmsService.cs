using System;
using System.Threading.Tasks;
using Infra.Shared.Configuration;
using Infra.SmsProvider.Kavenegar.Services.Interfaces;
using Kavenegar;
using Kavenegar.Models.Enums;
using Kave = Kavenegar;

namespace Infra.SmsProvider.Kavenegar.Services;

public class KavenegarSmsService : IKavenegarSmsService
{
    private readonly Kave.KavenegarApi _api;

    public KavenegarSmsService(SmsConfiguration smsConfiguration)
    {
        _api = new KavenegarApi(smsConfiguration.Provider.Kavenegar.ApiKey);
    }

    public Task SendMessage(string phoneNumber, string template, params string[] tokens)
    {
        //token is required, only for non-space text
        //token2 not-required, only for non-space text
        //token3 not-required, only for non-space text
        //token10 > required if you need the text that should contain 4 spaces 
        //token20 > required if you need the text that should contain 8 spaces 
        
        if (tokens.Length == 0)
            throw new ArgumentNullException();

        else  if (tokens.Length == 1)
             _api.VerifyLookup(phoneNumber, tokens[0], template);

        else   if (tokens.Length == 2)
             _api.VerifyLookup(phoneNumber, tokens[0], null, null, tokens[1], template);

        else  if (tokens.Length == 3)
             _api.VerifyLookup(phoneNumber, tokens[0], null, null, tokens[1], tokens[2], template, VerifyLookupType.Sms);

        else
            throw new OverflowException();
        
        return Task.CompletedTask;
    }
}