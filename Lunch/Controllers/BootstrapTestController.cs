using System.Web.Mvc;

namespace Lunch.Controllers
{
    public class BootstrapTestController : Controller
    {
        // GET: BootstrapTest
        public ActionResult Index()
        {
            // var res = LunchRepository.GetPushedRestaurant();

            //return View(res);
            return View();
        }

        [HttpPost]
        public ActionResult Index(string data)
        {
            return View();
        }
    }
}