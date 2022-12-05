using Lunch.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Dapper;
using Lunch.Helper;

namespace Lunch.Controllers
{
    public class EditOrderController : SettleBaseController
    {
        // GET: EditOrder
        public ActionResult Index(string ResName)
        {
            if (ResName != null)
            {
                try
                {
                    var Res = LunchRepository.GetRestaurantByName(ResName);
                    var TotalOrder2 = LunchRepository.GetAllOrder2(Res.RestaurantId);
                    var notSettledRes2 = LunchRepository.GetNotSettledRes();
                    ViewBag.pushrestel = Res.Telephone;
                    ViewBag.pushresurl = Res.MenuUrl;
                    return
                        View(new TotalViewModel
                        {
                            Orders = TotalOrder2,
                            Username = LunchUser.Name,
                            ResName = Res.Name,
                            IsAdmin = LunchUser.Admin,
                            notSettledRes = notSettledRes2,
                            IsClose = Res.IsClose
                        });
                }
                catch (Exception e)
                {
                    LogHelper.Log(string.Format("{0}{1}", e.Message, e.StackTrace));
                }
            }

            var Push = LunchRepository.GetPushedRestaurant().First(); // 8/10 15:00
            var TotalOrder = LunchRepository.GetAllOrder2(Push.RestaurantId);
            //var TotalOrder = LunchRepository.GetAllOrder();
            var notSettledRes = LunchRepository.GetNotSettledRes();
            ViewBag.pushrestel = Push.Telephone;
            ViewBag.pushresurl = Push.MenuUrl;
            var model = new TotalViewModel
            {
                Orders = TotalOrder,
                Username = LunchUser.Name,
                ResName = Push.Name,
                IsAdmin = LunchUser.Admin,
                IsClose = Push.IsClose,
                notSettledRes = notSettledRes
            };
            return View(model);
        }


        [HttpPost]
        public ActionResult Change(List<string> name, List<string> meal, List<string> cost, List<string> customerId,
            List<string> orderId) //要再加入OrderId以避免洗掉重複的點餐
        {
            //List<OrderInfo> Total = LunchRepository.GetAllOrder().ToList<OrderInfo>();//直接從GetAllOrder()的SP中拿data出來，所以線上改的資料無法傳回後端
            if (orderId != null)
            {
                var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]);
                for (int i = 0; i < meal.Count; i++)
                {
                    connection.Execute(
                        "UPDATE [Order] SET [Meal]=@O_meal,[Cost]=@costs,[Time]=@time Where [CustomerId]=@C_id AND [OrderId]=@O_id",
                        new
                        {
                            O_meal = meal[i],
                            costs = cost[i],
                            time = DateTime.Today,
                            C_id = customerId[i],
                            O_id = orderId[i]
                        }); //將修改的點餐加入Order的table，利用DateTime.Now把C#現在的時間加到SQL中
                }
            }

            return RedirectToAction("Index", "TotalCost");
        }

        public class TotalViewModel
        {
            public IEnumerable<OrderInfo> Orders { get; set; }
            public string Username { get; set; }
            public string ResName { get; set; }
            public bool IsAdmin { get; set; }
            public bool IsClose { get; set; }
            public IEnumerable<Restaurant> notSettledRes { get; set; }
        }

        [HttpPost]
        public ActionResult DeleteOrder(string orderId)
        {
            LunchRepository.DeleteOrder(Int32.Parse(orderId));
            return null;
        }

        [HttpPost]
        public ActionResult SelectRes(string ResName)
        {
            var Res = LunchRepository.GetRestaurantByName(ResName);
            var TotalOrder = LunchRepository.GetAllOrder2(Res.RestaurantId);
            var notSettledRes = LunchRepository.GetNotSettledRes();
            ViewBag.pushrestel = Res.Telephone;
            ViewBag.pushresurl = Res.MenuUrl;
            return
                View(new TotalViewModel
                {
                    Orders = TotalOrder,
                    Username = LunchUser.Name,
                    ResName = Res.Name,
                    IsAdmin = LunchUser.Admin,
                    notSettledRes = notSettledRes
                });
        }
    }
}