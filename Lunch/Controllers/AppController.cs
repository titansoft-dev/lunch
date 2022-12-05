using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Lunch.Entity;
using Lunch.Service;

namespace Lunch.Controllers
{
    public class AppController : BaseController
    {
        private EncryptProcessor encryptor = new EncryptProcessor();
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult LoginFromApp(string username, string password)
        {
            password = encryptor.getEncryptedStr(password);
            var login = LunchRepository.GetLoginInformation(username, password);
            if (login == default(Customer))
                return Json(new JsonResponse {IsSuccessful = false, Data = null});
            login.Balance = LunchRepository.RecentBalance(login.Name);

            return Json(new JsonResponse
            {
                IsSuccessful = true,
                Data = new LoginResponse
                {
                    Customer = login,
                    Orders = GetOrderList().ToList()
                }
            });

        }

        [HttpPost]
        [AllowCrossSiteJson]
        public IEnumerable<OrderInfo> GetOrderList()
        {
            var orderList = LunchRepository.GetAllOrder2(LunchRepository.GetPushedRestaurant().First().RestaurantId);
            return orderList;
        }
    }
}