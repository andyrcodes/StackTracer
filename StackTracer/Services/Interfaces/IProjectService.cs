using StackTracer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Services.Interfaces
{
    public interface IProjectService
    {
        public Task<bool> IsUserOnProject(string userId, int projectId);

        public Task<IEnumerable<Project>> ListUserProjects(string userId);

        public Task AddUserToProject(string userId, int projectId);

        public Task RemoveUserFromProject(string userId, int projectId);

        public Task<IEnumerable<AppUser>> UsersOnProject(int projectId);

        public Task<IEnumerable<AppUser>> UsersNotOnProject(int projectId);

        public Task<IEnumerable<AppUser>> DevelopersOnProject(int projectId);

        public Task<IEnumerable<AppUser>> SubmittersOnProject(int projectId);

        public Task<AppUser> ProjectManagerOnProject(int projectId);

    }
}
