using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Web.Controllers
{
    [Authorize(Roles = "Worker")]
    public class WorkerDashboardController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult IndexAr() => View();
    }
}
