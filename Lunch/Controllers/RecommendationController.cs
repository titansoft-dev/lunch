using System.Web.Mvc;

namespace Lunch.Controllers
{
    public class RecommendationController : UserBaseController
    {
        // GET: Recommendation
        public ActionResult Index()
        {
            return View();
        }
    }
}