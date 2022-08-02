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
    public class AccessService : IAccessService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppUser _user;

        public AccessService(ApplicationDbContext context, IHttpContextAccessor httpContext, UserManager<AppUser> userManager)
        {
            _context = context;
            _httpContext = httpContext;
            _userManager = userManager;
            _user = _context.Users.Find(_userManager.GetUserId(_httpContext.HttpContext.User));
        }

        public async Task<bool> CanInteractAttachment(Attachment attachment)
        {
            var roles = await _userManager.GetRolesAsync(_user);
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
            bool result = false;
            switch (role)
            {
                case "Admin":
                    result = true;
                    break;
                case "ProjectManager":
                    var projectId = (await _context.Tickets.FindAsync(attachment.TicketId)).ProjectId;
                    var project = await _context.Projects.FindAsync(projectId);
                    if (project.ProjectManagerId == _user.Id || await _context.Tickets.Where(t => t.OwnerUserId == _user.Id && t.Id == attachment.TicketId).AnyAsync() || attachment.UserId == _user.Id)
                    {
                        result = true;
                    }
                    break;
                case "Developer":
                    if (attachment.UserId == _user.Id)
                    {
                        result = true;
                    }
                    break;
                case "Submitter":
                    if (attachment.UserId == _user.Id)
                    {
                        result = true;
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        public async Task<bool> CanInteractComment(Comment comment)
        {
            var roles = await _userManager.GetRolesAsync(_user);
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
            bool result = false;
            switch (role)
            {
                case "Admin":
                    result = true;
                    break;
                case "ProjectManager":
                    var projectId = (await _context.Tickets.FindAsync(comment.TicketId)).ProjectId;
                    var project = await _context.Projects.FindAsync(projectId);
                    if (project.ProjectManagerId == _user.Id || await _context.Tickets.Where(t => t.OwnerUserId == _user.Id && t.Id == comment.TicketId).AnyAsync() || comment.UserId == _user.Id)
                    {
                        result = true;
                    }
                    break;
                case "Developer":
                    if (comment.UserId == _user.Id)
                    {
                        result = true;
                    }
                    break;
                case "Submitter":
                    if (comment.UserId == _user.Id)
                    {
                        result = true;
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        public async Task<bool> CanInteractProject(int projectId)
        {
            var roles = await _userManager.GetRolesAsync(_user);
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
                    return true;
                case "ProjectManager":
                    var project = await _context.Projects.FindAsync(projectId);
                    if (project.ProjectManagerId == _user.Id)
                    {
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        public async Task<bool> CanInteractTicket(int ticketId)
        {
            var roles = await _userManager.GetRolesAsync(_user);
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
            bool result = false;
            switch (role)
            {
                case "Admin":
                    result = true;
                    break;
                case "ProjectManager":
                    var projectId = (await _context.Tickets.FindAsync(ticketId)).ProjectId;
                    var project = await _context.Projects.FindAsync(projectId);
                    if (project.ProjectManagerId == _user.Id || await _context.Tickets.Where(t => t.OwnerUserId == _user.Id && t.Id == ticketId).AnyAsync())
                    {
                        result = true;
                    }
                    break;
                case "Developer":
                    if (await _context.Tickets.Where(t => (t.DeveloperUserId == _user.Id || t.OwnerUserId == _user.Id) && t.Id == ticketId).AnyAsync())
                    {
                        result = true;
                    }
                    break;
                case "Submitter":
                    if (await _context.Tickets.Where(t => t.OwnerUserId == _user.Id && t.Id == ticketId).AnyAsync())
                    {
                        result = true;
                    }
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
