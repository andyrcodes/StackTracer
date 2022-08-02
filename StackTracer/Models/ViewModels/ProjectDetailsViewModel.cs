using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Models.ViewModels
{
    public class ProjectDetailsViewModel
    {
        public Project Project { get; set; }

        public List<Ticket> Tickets { get; set; }
    }
}
