using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Lunch.Entity;
using System.Data.SqlClient;
using Dapper;
using Newtonsoft.Json;


namespace Lunch.Controllers
{
    public class TotalCostController : UserBaseController //SettleBaseController
    {
        // GET: TotalCost
        public ActionResult Index()
        {
            var usersession = Session["User"] as Customer;
            //if (usersession == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            var Push = LunchRepository.GetPushedRestaurant().First();
            var OrderInfo = LunchRepository.GetTotalOrderInformation2(Push.RestaurantId);
            //由Push.RestaurantId拿到當日的restaurantID，再送進GetTotalOrderInformation取出restaurantID相符的點餐記錄
            var meal = LunchRepository.GroupByMeal2(Push.RestaurantId);
            var haveNotSettleRes = LunchRepository.GetNotSettledRes();
            var Ordertime = LunchRepository.OrderTime(Push.RestaurantId); //Ordertime抓出點餐的日期，但是[Settle] = 'false'的不抓
            if (Ordertime != null) //Push.RestaurantId != 54) 
            {
                ViewBag.OTime = Ordertime.Time.ToString("yyyy/MM/dd"); //抓目前顯示餐點，點餐時的日期 
            }
            ViewBag.admin = usersession.Admin;
            ViewBag.pushrestel = Push.Telephone;
            ViewBag.pushresurl = Push.MenuUrl;

            return
                View(new SumUpViewModel
                {
                    groupmeal = meal,
                    orderinfo = OrderInfo,
                    Admin = LunchUser.Admin,
                    notSettledRes = haveNotSettleRes,
                    IsClose = Push.IsClose
                });
        }

        [HttpPost]
        public ActionResult index()
        {
            var usersession = Session["User"] as Customer;
            if (usersession.Admin)
            {
                var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]);
                var Push = LunchRepository.GetPushedRestaurant().First(); // 8/10 15:00
                var Total = LunchRepository.GetAllOrder2(Push.RestaurantId);
                var notordername = LunchRepository.GetNotOrder2(Push.RestaurantId);

                foreach (var notorder in notordername) //當日沒有點餐的人還是會insert進入資料庫 meal=notordering
                {
                    var resentbalance =
                        connection.Query<int>(
                            "SELECT [Balance] FROM [Summary] WHERE [CustomerId]=@C_Id ORDER BY [SummaryId] desc",
                            new { C_Id = notorder.CustomerId }).FirstOrDefault();
                    connection.Execute(
                        "INSERT INTO [Summary]([Meal],[Cost],[Time],[Name],[CustomerId],[RestaurantId],[Balance]) values(@SMeal,@SCost,@STime,@SName,@SCustomerId,@SRestaurantId,@SBalance)",
                        new
                        {
                            SMeal = "Not ordering",
                            SCost = 0,
                            STime = DateTime.Now,
                            SName = notorder.Name,
                            SCustomerId = notorder.CustomerId,
                            SRestaurantId = Push.RestaurantId,
                            SBalance = resentbalance
                        });
                }
                foreach (var each in Total) //當日點餐的人insert進入資料庫
                {
                    var resentbalance =
                        connection.Query<int>(
                            "SELECT [Balance] FROM [Summary] WHERE [CustomerId]=@C_Id ORDER BY [SummaryId] desc",
                            new { C_Id = each.CustomerId }).FirstOrDefault();
                    connection.Execute(
                        "INSERT INTO [Summary]([Meal],[Cost],[Time],[Name],[CustomerId],[RestaurantId],[Balance]) values(@SMeal,@SCost,@STime,@SName,@SCustomerId,@SRestaurantId,@SBalance)",
                        new
                        {
                            SMeal = each.Meal,
                            SCost = -each.Cost,
                            STime = DateTime.Now,
                            SName = each.Name,
                            SCustomerId = each.CustomerId,
                            SRestaurantId = Push.RestaurantId,
                            SBalance = resentbalance - each.Cost
                        });
                    LunchRepository.AddIntoRecommendation(Push.RestaurantId, each);
                }
                LunchRepository.UpdateSettleByRid(Push.RestaurantId); //選定餐廳中settle=false的更改為true
                connection.Execute("UPDATE [Restaurant] SET [Ispushed]=@trueValue WHERE [RestaurantId]=@restaurantId",
                    new { trueValue = false, restaurantId = Push.RestaurantId }); //Settle按下，不顯示餐廳menu
                connection.Dispose();
            }

            return RedirectToAction("Index", "TotalCost");
        }

        [HttpPost]
        public ActionResult WatchDetail(int? Rid)
        {
            var pastRestaurant = LunchRepository.GetRestaurantById(Rid.Value);
            var detail = LunchRepository.GetTotalOrderInformation2(Rid.Value);

            var pastdetail = new PastDetail();
            pastdetail.detail = detail;
            pastdetail.pastRestaurant = pastRestaurant;
            pastdetail.totalAmount = detail.Sum(x => x.Cost);
            pastdetail.summaryOrder = GetSummaryOrder(detail);
            pastdetail.totalMeal = detail.Count();

            var pastDetailJson = JsonConvert.SerializeObject(pastdetail);
            return Json(pastDetailJson);
        }

        private Dictionary<string, int> GetSummaryOrder(IEnumerable<OrderInfo> detail)
        {
            var summary = from p in detail
                group p by new {Meal = p.Meal.Trim()}
                into g
                select new
                {
                    Name = g.Key.Meal,
                    totalCount = g.Count()
                };
            var summaryOrder = summary.OrderByDescending(x => x.totalCount).ToDictionary(x => x.Name, x => x.totalCount);
            return summaryOrder;
        }

        public class PastDetail
        {
            public Restaurant pastRestaurant { get; set; }
            public IEnumerable<OrderInfo> detail { get; set; }
            public Dictionary<string,int> summaryOrder { get; set; }
            public double totalAmount { get; set; }
            public int totalMeal { get; set; }
    }

        public ActionResult SettlePastRes(int? Rid)
        {
            if (LunchUser.Admin)
            {
                var pushedRes = LunchRepository.GetPushedRestaurant().First();
                var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]);
                var Total = LunchRepository.GetAllOrder2(Rid.Value);
                var notordername = LunchRepository.GetNotOrder2(Rid.Value);

                foreach (var notorder in notordername) //當日沒有點餐的人還是會insert進入資料庫 meal=notordering
                {
                    var resentbalance =
                        connection.Query<int>(
                            "SELECT [Balance] FROM [Summary] WHERE [CustomerId]=@C_Id ORDER BY [SummaryId] desc",
                            new { C_Id = notorder.CustomerId }).FirstOrDefault();
                    connection.Execute(
                        "INSERT INTO [Summary]([Meal],[Cost],[Time],[Name],[CustomerId],[RestaurantId],[Balance]) values(@SMeal,@SCost,@STime,@SName,@SCustomerId,@SRestaurantId,@SBalance)",
                        new
                        {
                            SMeal = "Not ordering",
                            SCost = 0,
                            STime = DateTime.Now,
                            SName = notorder.Name,
                            SCustomerId = notorder.CustomerId,
                            SRestaurantId = Rid.Value,
                            SBalance = resentbalance
                        });
                }

                foreach (var each in Total) //當日點餐的人insert進入資料庫
                {
                    var resentbalance =
                        connection.Query<int>(
                            "SELECT [Balance] FROM [Summary] WHERE [CustomerId]=@C_Id ORDER BY [SummaryId] desc",
                            new { C_Id = each.CustomerId }).FirstOrDefault();
                    connection.Execute(
                        "INSERT INTO [Summary]([Meal],[Cost],[Time],[Name],[CustomerId],[RestaurantId],[Balance]) values(@SMeal,@SCost,@STime,@SName,@SCustomerId,@SRestaurantId,@SBalance)",
                        new
                        {
                            SMeal = each.Meal,
                            SCost = -each.Cost,
                            STime = DateTime.Now,
                            SName = each.Name,
                            SCustomerId = each.CustomerId,
                            SRestaurantId = Rid.Value,
                            SBalance = resentbalance - each.Cost
                        });
                }
                LunchRepository.UpdateSettleByRid(Rid.Value); //選定餐廳中settle=false的更改為true
                connection.Execute("UPDATE [Restaurant] SET [Ispushed]=@trueValue WHERE [RestaurantId]=@restaurantId", new { trueValue = false, restaurantId = Rid.Value }); 
                connection.Dispose();
            }
            return RedirectToAction("Index", "TotalCost");
        }

        public ActionResult DeleteNotSettledRes(int? Rid)
        {
            LunchRepository.UpdateSettleByRid(Rid.Value);
            return RedirectToAction("Index", "TotalCost");
        }

        public ActionResult CloseOrder()
        {
            var usersession = Session["User"] as Customer;
            if (usersession.Admin)
            {
                var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]);
                var Push = LunchRepository.GetPushedRestaurant().First();
                connection.Execute("UPDATE [Restaurant] SET [IsClose]= 1 WHERE [RestaurantId]=@restaurantId", new { restaurantId =Push.RestaurantId});
            }
            return RedirectToAction("Index", "totalCost");
        }
        public ActionResult CloseOrderRes(int? Rid)
        {
            var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]);
            connection.Execute("UPDATE [Restaurant] SET [IsClose]= 1 WHERE [RestaurantId]=@restaurantId", new { restaurantId = Rid.Value });
            return RedirectToAction("Index", "totalCost");
        }
        public ActionResult OpenOrder()
        {
            var usersession = Session["User"] as Customer;
            if (usersession.Admin)
            {
                var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]);
                var Push = LunchRepository.GetPushedRestaurant().First();
                connection.Execute("UPDATE [Restaurant] SET [IsClose]= 0 WHERE [RestaurantId]=@restaurantId", new { restaurantId = Push.RestaurantId });
            }
            return RedirectToAction("Index", "totalCost");
        }
        public ActionResult OpenOrderRes(int? Rid)
        {
            var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]);
            connection.Execute("UPDATE [Restaurant] SET [IsClose]= 0 WHERE [RestaurantId]=@restaurantId", new { restaurantId = Rid.Value });
            return RedirectToAction("Index", "totalCost");
        }
    }

    public class SumUpViewModel
    {
        public IEnumerable<OrderInfo> getorder { get; set; }
        public IEnumerable<OrderInfo> groupmeal { get; set; } //getOrder為Model名稱
        public IEnumerable<OrderInfo> orderinfo { get; set; } //orderinfo自訂名稱
        public IEnumerable<Restaurant> notSettledRes { get; set; }

        public bool IsClose { get; set; }
        public bool Admin { get; set; }
    }
}