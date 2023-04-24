using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infra.Shared.Services.Hubs
{
    public class SignalRSender : ISignalRSender
    {
        private Microsoft.AspNetCore.SignalR.IHubContext<SignalRDataHub> _signalRDataHub;
        public SignalRSender(Microsoft.AspNetCore.SignalR.IHubContext<SignalRDataHub> signalRDataHub)
        {
            _signalRDataHub = signalRDataHub;
        }

        public async Task Send(int id, List<string> subscriberUserIds, string notificationType, string notificationEvent, object notificationData)
        {
            foreach (var subscriberUserId in subscriberUserIds)
            {
                await _signalRDataHub.Clients.User(subscriberUserId.ToString()).SendCoreAsync(notificationType, new object[] { new {
                    Id = id,
                    NotificationType = notificationType,
                    NotificationEvent = notificationEvent,
                    NotificationData = notificationData
                } });
            }
        }
    }
}
