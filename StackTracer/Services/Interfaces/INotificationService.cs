using StackTracer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StackTracer.Services.Interfaces
{
    public interface INotificationService
    {
        //public Task CheckNotificationsAsync(ClaimsPrincipal user, bool isLogin);

        public Task CreateNotification(int ticketId, string description, string recipientId, string senderId);

        public Task<IEnumerable<Notification>> GetUnreadNotifications(ClaimsPrincipal user);

        public Task<int> NewTicketCount(ClaimsPrincipal currentUser);
        public Task<int> CriticalTicketCount(ClaimsPrincipal currentUser);
        public Task<int> MoreInfoTicketCount(ClaimsPrincipal currentUser);
        public Task<int> RecentTicketCount(ClaimsPrincipal currentUser);
        public Task<int> ResolvedTicketCount(ClaimsPrincipal currentUser);
    }
}
