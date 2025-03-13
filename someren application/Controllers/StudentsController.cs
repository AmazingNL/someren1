using Microsoft.AspNetCore.Mvc;

namespace someren_application.Controllers
{
    public class StudentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
