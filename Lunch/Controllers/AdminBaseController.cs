using System.Web.Mvc;

namespace Lunch.Controllers
{
    public class AdminBaseController : BaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (LunchUser.CustomerId == -1)
            {
                filterContext.Result = new RedirectResult(Url.Action("Index", "Login"));
                base.OnActionExecuting(filterContext);
                return;
            }

            if (!LunchUser.Admin)
            {
                filterContext.Result = new RedirectResult(Url.Action("Index", "ClientOrder"));
                base.OnActionExecuting(filterContext);
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}