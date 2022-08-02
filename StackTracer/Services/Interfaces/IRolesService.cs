using StackTracer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Services.Interfaces
{
    public interface IRolesService
    {
        public Task<bool> AddUserToRole(AppUser user, string roleName);

        public Task<bool> IsUserInRole(AppUser user, string roleName);

        public Task<IEnumerable<string>> ListUserRoles(AppUser user);

        public Task<bool> RemoveUserFromRole(AppUser user, string roleName);

        public Task<IEnumerable<AppUser>> UsersInRole(string roleName);

        public Task<IEnumerable<AppUser>> UsersNotInRole(string roleName);

        public IEnumerable<string> NonDemoRoles();
    }
}
