using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackTracer.Data;
using StackTracer.Enums;
using StackTracer.Models;
using StackTracer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Services
{
    public class RolesService : IRolesService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public RolesService(
            UserManager<AppUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> AddUserToRole(AppUser user, string roleName)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<bool> IsUserInRole(AppUser user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<IEnumerable<string>> ListUserRoles(AppUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public IEnumerable<string> NonDemoRoles()
        {

            var roles = _context.Roles;
            var output = new List<string>();
            foreach (var role in roles)
            {
                if (role.Name != "Demo")
                {
                    output.Add(role.Name);
                }
            }
            return output;
        }

        public async Task<string> NonDemoUserRole(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            var output = "";
            foreach (var role in roles)
            {
                if (role != Roles.Demo.ToString())
                {
                    output = role;
                }
            }
            return output;
        }

        public async Task<bool> RemoveUserFromRole(AppUser user, string roleName)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<IEnumerable<AppUser>> UsersInRole(string roleName)
        {
            return await _userManager.GetUsersInRoleAsync(roleName);
        }

        public async Task<IEnumerable<AppUser>> UsersNotInRole(string roleName)
        {
            var inRole = await _userManager.GetUsersInRoleAsync(roleName);
            var users = await _userManager.Users.ToListAsync();
            return users.Except(inRole);
        }
    }
}
