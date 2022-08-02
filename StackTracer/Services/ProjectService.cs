using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackTracer.Data;
using StackTracer.Models;
using StackTracer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRolesService _rolesService;

        public ProjectService(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            IHttpContextAccessor contextAccessor,
            IRolesService rolesService)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _rolesService = rolesService;
        }

        public async Task<bool> IsUserOnProject(string userId, int projectId)
        {
            if (await _userManager.IsInRoleAsync(await _userManager.FindByIdAsync(userId), "Admin"))
            {
                return true;
            }
            if (await _userManager.IsInRoleAsync(await _userManager.FindByIdAsync(userId), "ProjectManager"))
            {
                return await _context.Projects.Where(p => p.Id == projectId && p.ProjectManagerId == userId).AnyAsync();
            }
            var user = await _userManager.FindByIdAsync(userId);
            return await _context.Projects.Include(p=> p.Users).Where(p => p.Id == projectId && p.Users.Contains(user)).AnyAsync();
        }

        public async Task<IEnumerable<Project>> ListUserProjects(string userId)
        {
            if (_contextAccessor.HttpContext.User.IsInRole("Admin"))
            {
                return await _context.Projects.ToListAsync();
            }
            if (_contextAccessor.HttpContext.User.IsInRole("ProjectManager"))
            {
                return await _context.Projects.Where(p => p.ProjectManagerId == userId).ToListAsync();
            }
            var user = await _userManager.FindByIdAsync(userId);
            List<Project> projects = await _context.Projects.Where(p => p.Users.Contains(user)).ToListAsync();
            return projects;
        }

        public async Task AddUserToProject(string userId, int projectId)
        {
            if (!await IsUserOnProject(userId, projectId))
            {
                try
                {
                    var project = await _context.Projects.FindAsync(projectId);
                    var user = await _userManager.FindByIdAsync(userId);

                    if (_contextAccessor.HttpContext.User.IsInRole("ProjectManager") || (await _rolesService.IsUserInRole(user, "ProjectManager")))
                    {
                        project.ProjectManagerId = userId;
                    }


                    project.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("*********** An Error Occurred When Adding the User to the Project **************");
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public async Task RemoveUserFromProject(string userId, int projectId)
        {
            if (await IsUserOnProject(userId, projectId))
            {
                try
                {
                    var project = await _context.Projects.FindAsync(projectId);
                    var user = await _userManager.FindByIdAsync(userId);

                    if (_contextAccessor.HttpContext.User.IsInRole("ProjectManager") || (await _rolesService.IsUserInRole(user, "ProjectManager")))
                    {
                        project.ProjectManagerId = "";
                    }


                    project.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("*********** An Error Occurred When Removing the User from the Project **************");
                    Debug.WriteLine(ex.Message);
                }

            }
        }

        public async Task<IEnumerable<AppUser>> UsersNotOnProject(int projectId)
        {

            var output = new List<AppUser>();
            foreach (var user in await _context.Users.ToListAsync())
            {
                if (!await IsUserOnProject(user.Id, projectId))
                {
                    output.Add(user);
                }
            }
            return output;

        }

        public async Task<IEnumerable<AppUser>> UsersOnProject(int projectId)
        {
            var output = new List<AppUser>();
            foreach (var user in await _context.Users.ToListAsync())
            {
                if (await IsUserOnProject(user.Id, projectId))
                {
                    output.Add(user);
                }
            }
            return output;

        }

        public async Task<AppUser> ProjectManagerOnProject(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            var projectManager = await _userManager.FindByIdAsync(project.ProjectManagerId);
            return projectManager;
        }

        public async Task<IEnumerable<AppUser>> DevelopersOnProject(int projectId)
        {
            var developers = await _userManager.GetUsersInRoleAsync("Developer");
            var onProject = await UsersOnProject(projectId);
            var devsOnProject = new List<AppUser>();
            devsOnProject.AddRange(developers.Intersect(onProject).ToList());
            return devsOnProject;
        }

        public async Task<IEnumerable<AppUser>> SubmittersOnProject(int projectId)
        {
            var submitters = await _userManager.GetUsersInRoleAsync("Submitter");
            var onProject = await UsersOnProject(projectId);
            var subsOnProject = new List<AppUser>();
            subsOnProject.AddRange(submitters.Intersect(onProject).ToList());
            return subsOnProject;
        }
    }
}
