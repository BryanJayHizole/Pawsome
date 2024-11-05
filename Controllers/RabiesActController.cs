using Microsoft.AspNetCore.Mvc;

namespace Pawsome.Controllers
{
    public class RabiesActController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
