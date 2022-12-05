using System.Collections.Generic;
using System.Web.Mvc;
using Lunch.Entity;

namespace Lunch.Controllers
{
    public class ShowOrderDetailController : BaseController
    {
        // GET: ShowOrderDetail
        public ActionResult Index(int? Rid)
        {
            var restaurant = LunchRepository.GetRestaurantById(Rid.Value);
            var detail = LunchRepository.GetTotalOrderInformation2(Rid.Value);

            return View(new ShowOrderViewModel {restaurant = restaurant, detail = detail});
        }

        public class ShowOrderViewModel
        {
            public Restaurant restaurant { get; set; }
            public IEnumerable<OrderInfo> detail { get; set; }
        }
    }
}