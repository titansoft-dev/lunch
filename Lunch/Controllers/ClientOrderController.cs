using Dapper;
using Lunch.Entity;
using Lunch.Entity.ClientOrder;
using Lunch.Helper;
using Lunch.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Lunch.Controllers
{
    public class ClientOrderController : UserBaseController
    {
        // GET: clientOrder
        //public ActionResult Index(string LGstatusname , int LGstatusCustomerId)
        public ActionResult Index(string data, int? restaurantId)
        {
            //ViewBag.flag = 0;
            try //取得log的方式
            {
                var usersession = Session["User"] as Customer; //"User"為名稱，要與前面(LoginController)一樣，as後面加model名稱

                ViewBag.custname = usersession.Name;

                var restaurants = LunchRepository.GetPushedRestaurant();
                var currentRestaurant = getCurrentRestaurant(restaurantId, restaurants);

                var allOrder1 = LunchRepository.GetAllOrder2(currentRestaurant.RestaurantId); //ajax用
                var resentbalance = LunchRepository.RecentBalance(usersession.Name);
                ViewBag.balance = resentbalance;
                var lunchUser = LunchUser.Name;
                var isAdmin = LunchUser.Admin;
                var allCustomer = LunchRepository.GetCustomerName2(); //尚不確定
                var recommend = LunchRepository.GetRecommend(currentRestaurant.RestaurantId);

                if (data != null) //Ajax
                {
                    var jsonmodel = new ClientOrderModelView();
                    jsonmodel.currentRestaurant = currentRestaurant;
                    jsonmodel.allOrder = allOrder1;
                    jsonmodel.user = lunchUser; //前端FollowList接收判斷登入者
                    string jconvert = JsonConvert.SerializeObject(jsonmodel);
                    return Json(jconvert, JsonRequestBehavior.AllowGet); //一定要加 JsonRequestBehavior.AllowGet，不然不能動
                }

                var requestList = LunchRepository.GetTransferList().Where(x => x.RestaurantId == usersession.CustomerId).ToList();
                if (requestList.Any())
                    ViewData["TransferMsg"] = "You have " + requestList.Count() + " transfer request";

                return
                    View(new ClientOrderModelView
                    {
                        restaurants = restaurants,
                        currentRestaurant = currentRestaurant,
                        allOrder = allOrder1,
                        Admin = isAdmin,
                        allCustomer = allCustomer,
                        recommends = recommend
                    });
            }
            catch (Exception e)
            {
                LogHelper.Log(string.Format("{0}{1}", e.Message, e.StackTrace));
            }

            return View();
        }

        private Restaurant getCurrentRestaurant(int? restaurantId, List<Restaurant> restaurants)
        {
            return restaurants.FirstOrDefault(x => x.RestaurantId == restaurantId) ?? restaurants.First();
        }

        [HttpPost]
        public ActionResult Index(OrderForm form)
        {
            var customer = Session["User"] as Customer;
            if (customer.Admin == true) //Admin可以透過人名選擇誰要點什麼
            {
                var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]);
                int cost = Convert.ToInt32(form.inputcost);
                int rid = Convert.ToInt32(form.restaurantId);
                int cid = LunchRepository.GetCustomerIdByName(form.inputname);
                var resentbalance = LunchRepository.RecentBalance(form.inputname);
                //var check = Session["pass"] as OrderInfo;
                string check =
                    connection.Query<string>(
                        "SELECT [Meal] FROM [Order] WHERE [CustomerId]=@C_Id AND [Time]=@time AND [RestaurantId]=@O_rid",
                        new { C_Id = cid, @time = DateTime.Today, O_rid = rid }).FirstOrDefault();
                if (check == "PASS") //若client在目前的餐廳已經pass過一次，就不再讓他PASS進資料庫
                {
                    connection.Dispose();
                    return RedirectToAction("Index");
                }
                if (form.inputcost != 0 || form.inputorder != null)
                {
                    int category = connection.Query<int>("SELECT [Category] FROM [Restaurant] WHERE [RestaurantId]=@R_ID", new { R_ID = rid }).FirstOrDefault();
                    string Meal = form.inputorder;
                    if (category == 3)
                    {
                        if (form.inputsweetness != null || form.inputice != null)
                        {
                            Meal += "(" + form.inputsweetness + form.inputice + ")";
                        }
                    }
                    connection.Execute(
                        "INSERT INTO [Order]([Meal],[Cost],[Time],[CustomerId],[Settle],[RestaurantId]) values(@meal,@costs,@time,@C_Id,@O_settle,@O_rid)",
                        new
                        {
                            meal = Meal,
                            costs = cost,
                            time = DateTime.Now,
                            C_Id = cid,
                            O_settle = false,
                            O_rid = rid
                        }); //將點餐時間加入Order的table，利用DateTime.Now把C#現在的時間加到SQL中
                    TempData["OrderSuccess"] = "點餐成功!你的餐點為"+Meal;
                }
                if (form.inputcost == 0 && form.inputorder == null)
                {
                    connection.Execute(
                        "INSERT INTO [Order](Meal,Cost,Time,CustomerId,Settle,RestaurantId) values(@meal,@costs,@time,@C_Id,@O_psettle,@P_rid)",
                        new { meal = "PASS", costs = 0, time = DateTime.Now, C_Id = cid, @O_psettle = false, P_rid = rid });
                    TempData["OrderSuccess"] = "點餐成功!你的餐點為PASS";
                }
            }
            else //一般使用者的單餐方式
            {
                var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]);
                int cost = Convert.ToInt32(form.inputcost);
                int rid = Convert.ToInt32(form.restaurantId);
                var resentbalance = LunchRepository.RecentBalance(form.inputname);
                bool IsClose = connection.Query<bool>(
                    "SELECT [IsClose] FROM [Restaurant] WHERE [RestaurantId]=@O_rid",
                    new { O_rid = rid }).FirstOrDefault();
                if (IsClose)
                {
                    TempData["AlertMsg"] = "Order has Closed!";
                    return RedirectToAction("Index", new { restaurantId = form.restaurantId });
                }
                //var check = Session["pass"] as OrderInfo;
                string check =
                    connection.Query<string>(
                        "SELECT [Meal] FROM [Order] WHERE [CustomerId]=@C_Id AND [Time]=@time AND [RestaurantId]=@O_rid",
                        new { C_Id = customer.CustomerId, @time = DateTime.Today, O_rid = rid }).FirstOrDefault();
                if (check == "PASS") //若client在目前的餐廳已經pass過一次，就不再讓他PASS進資料庫
                {
                    connection.Dispose();
                    return RedirectToAction("Index");
                }
                if (resentbalance > -1000)
                {
                    if (form.inputcost != 0 || form.inputorder != null)
                    {
                        int category = connection.Query<int>("SELECT [Category] FROM [Restaurant] WHERE [RestaurantId]=@R_ID", new { R_ID = rid }).FirstOrDefault();
                        string Meal = form.inputorder;
                        if (category == 3)
                        {
                            if (form.inputsweetness != null || form.inputice != null)
                            {
                                Meal += "(" + form.inputsweetness + form.inputice + ")";
                            }
                        }
                        connection.Execute(
                            "INSERT INTO [Order]([Meal],[Cost],[Time],[CustomerId],[Settle],[RestaurantId]) values(@meal,@costs,@time,@C_Id,@O_settle,@O_rid)",
                            new
                            {
                                meal = Meal,
                                costs = cost,
                                time = DateTime.Now,
                                C_Id = customer.CustomerId,
                                O_settle = false,
                                O_rid = rid
                            }); //將點餐時間加入Order的table，利用DateTime.Now把C#現在的時間加到SQL中
                        TempData["OrderSuccess"] = "點餐成功!你的餐點為"+Meal;
                    }
                    if (form.inputcost == 0 && form.inputorder == null)
                    {
                        connection.Execute(
                            "INSERT INTO [Order](Meal,Cost,Time,CustomerId,Settle,RestaurantId) values(@meal,@costs,@time,@C_Id,@O_psettle,@P_rid)",
                            new
                            {
                                meal = "PASS",
                                costs = 0,
                                time = DateTime.Now,
                                C_Id = customer.CustomerId,
                                @O_psettle = false,
                                P_rid = rid
                            });
                        TempData["OrderSuccess"] = "點餐成功!你的餐點為PASS";
                    }
                }
                else
                {
                    TempData["alertMessage"] = "你已負債1000元，請先到OD儲值再來點餐!";
                }
            }
            return RedirectToAction("Index", new { restaurantId = form.restaurantId });
        }
    }
}