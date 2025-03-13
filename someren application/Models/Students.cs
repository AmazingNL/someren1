using Microsoft.AspNetCore.Mvc;

namespace someren_application.Models
{
    public class Students : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
