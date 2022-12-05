using System.Web.Mvc;
using System.Web.Routing;

namespace Lunch
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new {controller = "Login", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}