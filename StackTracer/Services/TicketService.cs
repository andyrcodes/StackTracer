using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackTracer.Data;
using StackTracer.Models;
using StackTracer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Services
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IProjectService _projectService;
        private readonly AppUser _user;

        public TicketService(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            IHttpContextAccessor contextAccessor,
            IProjectService projectService)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _projectService = projectService;
            _user = _context.Users.Find(_userManager.GetUserId(_contextAccessor.HttpContext.User));
        }

        public async Task<bool> IsUserOnTicket(Ticket ticket)
        {
            if (_contextAccessor.HttpContext.User.IsInRole("Admin"))
            {
                return true;
            }
            else if (_contextAccessor.HttpContext.User.IsInRole("ProjectManager"))
            {
                if (await _projectService.IsUserOnProject(_user.Id, ticket.ProjectId) || ticket.OwnerUserId == _user.Id)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return _context.Tickets.Where(t => (t.DeveloperUserId == _user.Id || t.OwnerUserId == _user.Id) && t.Id == ticket.Id).Any();
        }

        public async Task<IEnumerable<Ticket>> ListUserTickets()
        {
            if (_contextAccessor.HttpContext.User.IsInRole("Admin"))
            {
                return await _context.Tickets.ToListAsync();
            }
            else if (_contextAccessor.HttpContext.User.IsInRole("ProjectManager"))
            {
                var data = await _context.Projects.Where(p => p.ProjectManagerId == _user.Id).Select(p => p.Tickets).ToListAsync();
                List<Ticket> tickets = new List<Ticket>();
                foreach (var collection in data)
                {
                    tickets.AddRange(collection);
                }
                return tickets;
            }
            return await _context.Tickets.Where(t => t.DeveloperUserId == _user.Id || t.OwnerUserId == _user.Id).ToListAsync();

        }

        public async Task<IEnumerable<Ticket>> GetCriticalTicketsAsync()
        {
            var roles = await _userManager.GetRolesAsync(_user);
            var criticalId = _context.TicketPriorities.FirstOrDefault(t => t.Name == "Critical").Id;
            var role = "";
            if (roles.Count > 1)
            {
                foreach (var name in roles)
                {
                    if (name != "Demo")
                    {
                        role = name;
                    }
                }
            }
            else
            {
                role = roles.FirstOrDefault();
            }
            switch (role)
            {
                case "Admin":
                    return await _context.Tickets.Where(t => t.TicketPriorityId == criticalId).ToListAsync();
                case "ProjectManager":
                    var output = new List<Ticket>();
                    var dataPacket = await _context.Projects.Where(p => p.ProjectManagerId == _user.Id).Select(p => p.Tickets.Where(t => t.TicketPriorityId == criticalId)).ToListAsync();
                    foreach (var ticketSet in dataPacket)
                    {
                        output.AddRange(ticketSet);
                    }
                    return output;
                //return (IEnumerable<Ticket>)_context.ProjectUsers.Where(p => p.UserId == _user.Id).Select(p => p.Project).Select(p => p.Tickets.Where(t => (t.TicketPriorityId == criticalId) || (t.OwnerUserId == _user.Id))).ToList();
                case "Developer":
                    return await _context.Tickets.Where(t => t.TicketPriorityId == criticalId && (t.DeveloperUserId == _user.Id || t.OwnerUserId == _user.Id)).ToListAsync();
                case "Submitter":
                    return await _context.Tickets.Where(t => t.TicketPriorityId == criticalId && t.OwnerUserId == _user.Id).ToListAsync();
                default:
                    return new List<Ticket>();
            }

        }

        public async Task<IEnumerable<Ticket>> GetMoreInfoTicketsAsync()
        {
            var roles = await _userManager.GetRolesAsync(_user);
            var moreInfoId = _context.TicketStatuses.FirstOrDefault(t => t.Name == "More Information").Id;
            var role = "";
            if (roles.Count > 1)
            {
                foreach (var name in roles)
                {
                    if (name != "Demo")
                    {
                        role = name;
                    }
                }
            }
            else
            {
                role = roles.FirstOrDefault();
            }
            switch (role)
            {
                case "Admin":
                    return await _context.Tickets.Where(t => t.TicketStatusId == moreInfoId).ToListAsync();
                case "ProjectManager":
                    var output = new List<Ticket>();
                    var dataPacket = await _context.Projects.Where(p => p.ProjectManagerId == _user.Id).Select(p => p.Tickets.Where(t => t.TicketStatusId == moreInfoId)).ToListAsync();
                    foreach (var ticketSet in dataPacket)
                    {
                        output.AddRange(ticketSet);
                    }
                    return output;
                //return (IEnumerable<Ticket>)_context.ProjectUsers.Where(p => p.UserId == _user.Id).Select(p => p.Project).Select(p => p.Tickets.Where(t => (t.TicketStatusId == moreInfoId) || (t.OwnerUserId == _user.Id))).ToList();
                case "Developer":
                    return await _context.Tickets.Where(t => t.TicketStatusId == moreInfoId && (t.DeveloperUserId == _user.Id || t.OwnerUserId == _user.Id)).ToListAsync();
                case "Submitter":
                    return await _context.Tickets.Where(t => t.TicketStatusId == moreInfoId && t.OwnerUserId == _user.Id).ToListAsync();
                default:
                    return new List<Ticket>();
            }

        }

        public async Task<IEnumerable<Ticket>> GetNewTicketsAsync()
        {
            var roles = await _userManager.GetRolesAsync(_user);
            var newId = _context.TicketStatuses.FirstOrDefault(t => t.Name == "New").Id;
            var role = "";
            if (roles.Count > 1)
            {
                foreach (var name in roles)
                {
                    if (name != "Demo")
                    {
                        role = name;
                    }
                }
            }
            else
            {
                role = roles.FirstOrDefault();
            }
            switch (role)
            {
                case "Admin":
                    return await _context.Tickets.Where(t => t.TicketStatusId == newId).ToListAsync();
                case "ProjectManager":
                    var output = new List<Ticket>();
                    var dataPacket = await _context.Projects.Where(p => p.ProjectManagerId == _user.Id).Select(p => p.Tickets.Where(t => t.TicketStatusId == newId)).ToListAsync();
                    foreach (var ticketSet in dataPacket)
                    {
                        output.AddRange(ticketSet);
                    }
                    return output;
                //return (IEnumerable<Ticket>)_context.ProjectUsers.Where(p => p.UserId == _user.Id).Select(p => p.Project).Select(p => p.Tickets.Where(t => (t.TicketStatusId == newId) || (t.OwnerUserId == _user.Id))).ToList();
                case "Developer":
                    return await _context.Tickets.Where(t => t.TicketStatusId == newId && (t.DeveloperUserId == _user.Id || t.OwnerUserId == _user.Id)).ToListAsync();
                case "Submitter":
                    return await _context.Tickets.Where(t => t.TicketStatusId == newId && t.OwnerUserId == _user.Id).ToListAsync();
                default:
                    return new List<Ticket>();
            }

        }

        public async Task<IEnumerable<TicketStatus>> GetStatuses()
        {
            return await _context.TicketStatuses.ToListAsync();
        }

        public async Task<IEnumerable<TicketType>> GetTypes()
        {
            return await _context.TicketTypes.ToListAsync();
        }

        public async Task<IEnumerable<TicketPriority>> GetPriorities()
        {
            return await _context.TicketPriorities.ToListAsync();
        }
    }
}
