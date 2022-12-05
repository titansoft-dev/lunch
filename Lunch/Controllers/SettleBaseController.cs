using System.Configuration;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Linq;

namespace Lunch.Controllers
{
    public class SettleBaseController : UserBaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]); //
            var pushedRes = LunchRepository.GetPushedRestaurant();
            connection.Dispose(); //
            if (LunchUser.Admin)
            {
                return;
            }

            if (pushedRes.Any(x => x.RestaurantId == 54))
            {
                filterContext.Result = new RedirectResult(Url.Action("Index", "ClientOrder"));
                base.OnActionExecuting(filterContext);
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}