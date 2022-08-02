using StackTracer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Services.Interfaces
{
    public interface ITicketService
    {
        public Task<bool> IsUserOnTicket(Ticket ticket);

        public Task<IEnumerable<Ticket>> ListUserTickets();

        public Task<IEnumerable<Ticket>> GetNewTicketsAsync();

        public Task<IEnumerable<Ticket>> GetCriticalTicketsAsync();

        public Task<IEnumerable<Ticket>> GetMoreInfoTicketsAsync();

        public Task<IEnumerable<TicketStatus>> GetStatuses();

        public Task<IEnumerable<TicketType>> GetTypes();

        public Task<IEnumerable<TicketPriority>> GetPriorities();
    }
}
