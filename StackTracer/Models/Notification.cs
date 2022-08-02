using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public string Description { get; set; }

        public DateTimeOffset Created { get; set; }

        public string RecipientId { get; set; }

        public string SenderId { get; set; }

        public bool IsRead { get; set; }

        public virtual Ticket Ticket { get; set; }

        public virtual AppUser Recipient { get; set; }

        public virtual AppUser Sender { get; set; }
    }
}
