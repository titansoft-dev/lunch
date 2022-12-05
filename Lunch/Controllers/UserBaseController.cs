using System.Web.Mvc;

namespace Lunch.Controllers
{
    public class UserBaseController : BaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (LunchUser.CustomerId == -1 || LunchUser.Permission == 4)
            {
                filterContext.Result = new RedirectResult(Url.Action("Index", "Login"));
                base.OnActionExecuting(filterContext);
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}