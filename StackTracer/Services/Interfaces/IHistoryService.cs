using StackTracer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Services.Interfaces
{
    public interface IHistoryService
    {
        public Task AddHistory(Ticket oldTicket, Ticket newTicker, string userId);
    }
}
