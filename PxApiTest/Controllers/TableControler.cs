using Microsoft.AspNetCore.Mvc;

namespace PxApiTest.Controllers
{
    public class TableControler : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
