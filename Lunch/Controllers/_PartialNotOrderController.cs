using Lunch.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Lunch.Controllers
{
    public class _PartialNotOrderController : BaseController
    {
        // GET: _PartialNotOrder
        public ActionResult Index(int? restaurantId)
        {
            var Push = LunchRepository.GetPushedRestaurant();
            var currentRestaurantId = restaurantId ?? Push.First().RestaurantId;
            var NotOrderName = LunchRepository.GetNotOrder2(currentRestaurantId);
            
            ViewBag.IsSeletedRestaurant = currentRestaurantId;
            return View(new PartialNotOrderModel { notordername = NotOrderName, restaurants = Push }); 
        }

        public class PartialNotOrderModel{
            public IEnumerable<Customer> notordername { get; set; }
            public IEnumerable<Restaurant> restaurants { get; set; }
        }
    }

}