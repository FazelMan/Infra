using System.Collections;
using Infra.Shared.Ioc;

namespace Infra.SmsProvider.Rahyab.Services.Interfaces;

public interface IRahyabSmsService : ITransientDependency
{
    string Validate_Number(string Number);
    string Validate_Message(ref string Message, bool IsPersian);
    string C2Unicode(string Message);

    string[] SendSMS_Single(string Message, string DestinationAddress, string Number, string userName, string password,
        string IP_Send, string Company, bool IsFlash);

    string[] SendSMS_Batch(string Message, string[] DestinationAddress, string Number, string userName, string password,
        string IP_Send, string Company, bool IsFlash);

    string[] SendSMS_Batch_Devided(string Message, string[] DestinationAddress, string Number, string userName,
        string password, string IP_Send, string Company, bool IsPersian, bool IsFlash);

    ArrayList SendSMS_Batch_Full(string Message, string[] DestinationAddress, string Number, string userName,
        string password, string IP_Send, string Company, bool IsFlash, ref string SendStatus, ref string BatchID,
        ref string Amount, ref string chargingAmount);

    ArrayList SendSMS_Batch_Devided_Full(string Message, string[] DestinationAddress, string Number, string userName,
        string password, string IP_Send, string Company, bool IsPersian, bool IsFlash, ref string SendStatus,
        ref string BatchID, ref string Amount, ref string chargingAmount);

    string[] SendSMS_LikeToLike(string[] Message, string[] DestinationAddress, string Number, string userName,
        string password, string IP_Send, string Company);

    string[] SendSMS_LikeToLike(string[] destinationAddress, string[] Message, long smsNumber);

    ArrayList SendSMS_LikeToLike_Full(string[] Message, string[] DestinationAddress, string Number, string userName,
        string password, string IP_Send, string Company, ref string SendStatus, ref string BatchID, ref string Amount,
        ref string chargingAmount);

    ArrayList SendSMS_LikeToLike_Full_Devided(string[] Message, string[] DestinationAddress, string Number,
        string userName, string password, string IP_Send, string Company, ref string SendStatus, ref string BatchID,
        ref string Amount, ref string chargingAmount);

    string[] SendSingle(string reciver, string textMessage);
}