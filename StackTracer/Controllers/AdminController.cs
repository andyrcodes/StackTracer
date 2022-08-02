using StackTracer.Data;
using StackTracer.Models;
using StackTracer.Models.ViewModels;
using StackTracer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Controllers
{
    [Authorize(Roles = "Admin, ProjectManager")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRolesService _rolesService;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(
            ApplicationDbContext context,
            IRolesService rolesService,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _rolesService = rolesService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var model = new AdminDashboardViewModel
            {
                DevUsers = await _rolesService.UsersInRole("Developer"),
                SubUsers = await _rolesService.UsersInRole("Submitter"),
                ProjectPM = await _rolesService.UsersInRole("ProjectManager"),
                Roles = _rolesService.NonDemoRoles()
            };
            if (User.IsInRole("Admin"))
            {
                model.AllUsers = await _context.Users.ToListAsync();
                model.Priorities = await _context.TicketPriorities.ToListAsync();
                model.Statuses = await _context.TicketStatuses.ToListAsync();
                model.Types = await _context.TicketTypes.ToListAsync();
            }
            else
            {
                var filteredUsers = new List<AppUser>();
                filteredUsers.AddRange(await _rolesService.UsersInRole("Developer"));
                filteredUsers.AddRange(await _rolesService.UsersInRole("Submitter"));
                filteredUsers.AddRange(await _rolesService.UsersInRole("NewUser"));
                model.AllUsers = filteredUsers;
            }


            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddUser()
        {
            var model = new AddUserViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Dashboard));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(string id, string firstName, string lastName, string email, string role)
        {
            if (!User.IsInRole("Demo"))
            {
                var user = await _userManager.FindByIdAsync(id);
                if (!string.IsNullOrEmpty(firstName))
                {
                    user.FirstName = firstName;
                }
                if (!string.IsNullOrEmpty(lastName))
                {
                    user.LastName = lastName;
                }
                if (!string.IsNullOrEmpty(email))
                {
                    user.Email = email;
                    user.UserName = email;
                    user.NormalizedUserName = email.ToUpper();
                }
                if (!string.IsNullOrEmpty(role))
                {
                    foreach (var userRole in await _rolesService.ListUserRoles(user))
                    {
                        if (userRole != "Demo")
                        {
                            await _rolesService.RemoveUserFromRole(user, userRole);
                        }
                    }
                    await _rolesService.AddUserToRole(user, role);
                }
                return RedirectToAction("Dashboard");
            }
            else
            {
                return RedirectToAction("Dashboard");
            }
        }

        #region Add/Edit/Delete Type/Status/Priority
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddType(string name)
        {
            var type = new TicketType { Name = name };
            await _context.TicketTypes.AddAsync(type);
            await _context.SaveChangesAsync();
            return RedirectToAction("Settings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPriority(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var priority = new TicketPriority { Name = name };
                await _context.TicketPriorities.AddAsync(priority);
                await _context.SaveChangesAsync();
            }

            TempData["ErrorMessage"] = "Name field cannot be empty!";

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStatus(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var status = new TicketStatus { Name = name };
                await _context.TicketStatuses.AddAsync(status);
                await _context.SaveChangesAsync();
            }

            TempData["ErrorMessage"] = "Name field cannot be empty!";

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditType(int id, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var type = await _context.TicketTypes.FindAsync(id);
                type.Name = name;
                await _context.SaveChangesAsync();
            }

            TempData["ErrorMessage"] = "Name field cannot be empty!";

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPriority(int id, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var priority = await _context.TicketPriorities.FindAsync(id);
                priority.Name = name;
                await _context.SaveChangesAsync();
            }

            TempData["ErrorMessage"] = "Name field cannot be empty!";

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(int id, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var status = await _context.TicketStatuses.FindAsync(id);
                status.Name = name;
                await _context.SaveChangesAsync();
            }

            TempData["ErrorMessage"] = "Name field cannot be empty!";

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteType(int id)
        {
            var type = await _context.TicketTypes.FindAsync(id);
            _context.TicketTypes.Remove(type);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePriority(int id)
        {
            var priority = await _context.TicketPriorities.FindAsync(id);
            _context.TicketPriorities.Remove(priority);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            var status = await _context.TicketStatuses.FindAsync(id);
            _context.TicketStatuses.Remove(status);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard");
        }
        #endregion
    }
}
