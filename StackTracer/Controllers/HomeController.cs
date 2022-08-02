using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackTracer.Data;
using StackTracer.Models;
using StackTracer.Models.ViewModels;
using StackTracer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotificationService _notificationService;
        private readonly ApplicationDbContext _context;
        private readonly ITicketService _ticketService;

        public HomeController(
            ILogger<HomeController> logger,
            INotificationService notificationService,
            ApplicationDbContext context,
            ITicketService ticketService)
        {
            _logger = logger;
            _notificationService = notificationService;
            _context = context;
            _ticketService = ticketService;
        }

        //General access to the application
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        //My dashboard for after login
        public async Task<IActionResult> Dashboard()
        {
            var model = new DashboardViewModel
            {
                Tickets = await _context.Tickets.Include(t => t.DeveloperUser)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType).ToListAsync(),
                Projects = await _context.Projects.ToListAsync()
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
