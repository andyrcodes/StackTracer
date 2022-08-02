using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        #region Ticket Settings
        public IEnumerable<TicketPriority> Priorities { get; set; }

        public IEnumerable<TicketStatus> Statuses { get; set; }

        public IEnumerable<TicketType> Types { get; set; }
        #endregion

        #region Project Creation Wizard
        public IEnumerable<AppUser> DevUsers { get; set; }

        public IEnumerable<AppUser> SubUsers { get; set; }

        public IEnumerable<AppUser> ProjectPM { get; set; }
        #endregion

        #region User Roles and Management
        public IEnumerable<AppUser> AllUsers { get; set; }

        public IEnumerable<string> Roles { get; set; }
        #endregion    
    }
}
