using System.Web.Mvc;

namespace Lunch.Controllers
{
    public class LogOutController : Controller
    {
        // GET: LogOut
        public ActionResult Index()
        {
            Session.Remove("User");
            return RedirectToAction("Index", "Login");
        }
    }
}