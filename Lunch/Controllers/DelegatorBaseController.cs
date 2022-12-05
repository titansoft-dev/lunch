using System.Web.Mvc;

namespace Lunch.Controllers
{
    public class DelegatorBaseController : BaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (LunchUser.CustomerId == -1)
            {
                filterContext.Result = new RedirectResult(Url.Action("Index", "Login"));
                base.OnActionExecuting(filterContext);
                return;
            }


            if (LunchUser.Admin != true | LunchUser.Permission != 1)
            {
                filterContext.Result = new RedirectResult(Url.Action("Index", "Restaurant"));
                base.OnActionExecuting(filterContext);
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}