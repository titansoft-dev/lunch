using Dapper;
using Lunch.Entity;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Lunch.Controllers
{
    public class RestaurantController : AdminBaseController
    {
        private SqlConnection _connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]);

        [HttpGet]
        public ActionResult Index(int? categorySelectedId, int? restaurantId)
        {
            var notSettledRes = LunchRepository.GetNotSettledRes();
            var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]);
            ViewBag.restaurantId = restaurantId == null ? "null" : restaurantId.ToString();
            var NewMember =
                connection.Query("SELECT [CustomerID] FROM [Customer] WHERE [Permission]=3 ").FirstOrDefault();

            if (NewMember != null)
            {
                ViewBag.NewMember = 1;
            }
            else
            {
                ViewBag.NewMember = -1;
            }

            if (categorySelectedId != null)
            {
                var restaurants = LunchRepository.GetRestaurantByCategory(categorySelectedId.Value);
                foreach (var res in restaurants) //將comment的"換行"符號轉成<br/>
                {
                    if (res.Comment == null)
                    {
                    }
                    else
                    {
                        if (res.Comment.Contains("\r\n"))
                        {
                            res.Comment = res.Comment.Replace("\r\n", "<br />");
                        }
                    }
                }
                return
                    View(new RestaurantModelView
                    {
                        restaurant = restaurants,
                        notSettledRestaurant = notSettledRes,
                        CategoryId = categorySelectedId.Value
                    });
            }
            else
            {
                var restaurants = LunchRepository.GetRestaurantByCategory(1);
                foreach (var res in restaurants) //將comment的"換行"符號轉成<br/>
                {
                    if (res.Comment == null)
                    {
                    }
                    else
                    {
                        if (res.Comment.Contains("\r\n"))
                        {
                            res.Comment = res.Comment.Replace("\r\n", "<br />");
                        }
                    }
                }
                return
                    View(new RestaurantModelView
                    {
                        restaurant = restaurants,
                        notSettledRestaurant = notSettledRes,
                        CategoryId = 1
                    });
            }
        }

        public class RestaurantModelView
        {
            public IEnumerable<Restaurant> restaurant { get; set; }
            public IEnumerable<Restaurant> notSettledRestaurant { get; set; }
            public int CategoryId { get; set; }
        }

        [HttpPost]
        public ActionResult Push(int? restaurantId)
        {
            updatePushStatus(restaurantId, true);
            return RedirectToAction("Index", "Restaurant");
        }

        [HttpPost]
        public ActionResult UnPush(int? restaurantId)
        {
            updatePushStatus(restaurantId, false);
            return RedirectToAction("Index", "Restaurant");
        }

        private void updatePushStatus(int? restaurantId, bool isPushed)
        {
            _connection.Execute("UPDATE Restaurant SET IsClose = 0,Ispushed=@trueValue WHERE RestaurantId=@restaurantId",
                new { trueValue = isPushed, restaurantId });
            _connection.Dispose();
        }

        [HttpPost]
        public ActionResult LunchComing()
        {
            var client = new LineClient();
            client.SendMessage("午歜囉!");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteRestaurant(int? restaurantId, string menuUrl)
        {
            if (restaurantId != 54)
            {
                string[] menuStr = menuUrl.Split('\\');
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/images/Menus"), menuStr[menuStr.Length - 1]));
                LunchRepository.DeleteRestaurantById(restaurantId.Value);
            }
            return RedirectToAction("Index");
        }
    }
}