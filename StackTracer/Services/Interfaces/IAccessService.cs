using StackTracer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Services.Interfaces
{
    public interface IAccessService
    {
        public Task<bool> CanInteractProject(int projectId);

        public Task<bool> CanInteractTicket(int ticketId);

        public Task<bool> CanInteractAttachment(Attachment attachment);

        public Task<bool> CanInteractComment(Comment comment);

    }
}
