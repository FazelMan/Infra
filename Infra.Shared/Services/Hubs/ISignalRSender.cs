using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infra.Shared.Services.Hubs
{
    public interface ISignalRSender
    {
        Task Send(int id, List<string> subscriberUserIds, string notificationType, string notificationEvent, object notificationData);
    }
}
