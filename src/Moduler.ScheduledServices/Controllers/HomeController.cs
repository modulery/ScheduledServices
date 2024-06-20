using Microsoft.AspNetCore.Mvc;
using Moduler.KuveytTurk.Data.Services;
using Moduler.ScheduledServices.Models;
using System.Diagnostics;

namespace Moduler.ScheduledServices.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IRemoteToLocalService _remoteToLocalService;

        public HomeController(ILogger<HomeController> logger, IRemoteToLocalService remoteToLocalService)
        {
            _logger = logger;
            _remoteToLocalService = remoteToLocalService;
        }

        public IActionResult Index()
        {
            var last = _remoteToLocalService.GetLastActivity();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
