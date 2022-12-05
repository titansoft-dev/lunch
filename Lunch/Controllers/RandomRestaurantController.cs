using System.Web.Mvc;

namespace Lunch.Controllers
{
    public class RandomRestaurantController : AdminBaseController
    {
        // GET: RandomRestaurant
        public ActionResult Index()
        {
            var randrest = LunchRepository.SelectRid();
            return View(randrest);
        }
    }
}