using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string Body { get; set; }

        public DateTimeOffset Created { get; set; }

        public int TicketId { get; set; }

        public string UserId { get; set; }

        public virtual Ticket Ticket { get; set; }

        public virtual AppUser User { get; set; }
    }
}
