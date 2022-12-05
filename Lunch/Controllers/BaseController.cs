using System.Web.Mvc;
using Lunch.Entity;
using Lunch.Helper;

namespace Lunch.Controllers
{
    public class BaseController : Controller
    {
        protected Customer LunchUser
        {
            get
            {
                var customer = Session["User"] as Customer;
                return customer ?? new Customer {CustomerId = -1}; //??表示是NULL的話，new Customer { CustomerId = -1 };做這件事
            }
        }

        protected Repository.LunchRepository LunchRepository = new Repository.LunchRepository();

        [AllowCrossSiteJson]
        protected override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                LogHelper.Log(string.Format("{0}{1}", filterContext.Exception.Message,
                    filterContext.Exception.StackTrace));
                filterContext.ExceptionHandled = true;
                filterContext.Result = new RedirectResult(Url.Action("Index", "Error"));
            }
            base.OnException(filterContext);
        }
    }
}