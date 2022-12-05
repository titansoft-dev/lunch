using Dapper;
using Lunch.Entity;
using Lunch.Repository;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;

namespace Lunch.Controllers
{
    [RoutePrefix("api/SlackApi")]
    public class SlackApiController : System.Web.Http.ApiController
    {
        LunchRepository _lunchRepository = new LunchRepository();

        public JArray GetPushedRestaurant()
        {
            var restaurants = _lunchRepository.GetPushedRestaurant().Where(r => r.IsClose != true).ToList();
            var j_restaurant = new JArray();
            if (restaurants == null)
            {
                j_restaurant.Add(new JObject
                {
                    {"status",false },
                    {"message","目前沒有開放任何餐廳" },
                    {"rid",null },
                    {"name", null},
                    {"img", null},
                });
            }
            else
            {
                foreach (var restaurant in restaurants)
                {
                    j_restaurant.Add(new JObject
                    {
                        {"status",true },
                        {"message","" },
                        {"rid",restaurant.RestaurantId },
                        {"name", restaurant.Name},
                        {"img", HostingEnvironment.MapPath(restaurant.MenuUrl)},
                    });
                }
            }           
            return j_restaurant;
        }

        [HttpPost]
        public JArray UserBalance(Slack slack)
        {
            var j_userBalance = new JArray();
            var customer = _lunchRepository.GetCustomerBySalckId(slack.slackId);
            if (customer == null)
            {
                j_userBalance.Add(new JObject
                {
                    {"Status", false},
                    {"Message", "你尚未設定slackId"},
                    {"Balance",null }
                });
            }
            else
            {
                var resentbalance = _lunchRepository.RecentBalance(customer.Name);
                j_userBalance.Add(new JObject
                {
                    {"Status", true},
                    {"Message", "slackId設定成功"},
                    {"Balance",resentbalance }
                });
            }
            return j_userBalance;
        }

        public JArray GetRichestUser()
        {
            var richestUser = _lunchRepository.GetRichestUser();
            var j_richestUser = new JArray();
            foreach (var user in richestUser)
            {
                j_richestUser.Add(new JObject
                    {
                        {"Name", user.Name},
                        {"Balance", user.Balance},
                    });
            }
            return j_richestUser;
        }

        public JArray GetLiabilityUser()
        {
            var customerbalance = _lunchRepository.GetCustomerBalance2(0);
            var j_customerbalance = new JArray();
            foreach (var balance in customerbalance)
            {
                j_customerbalance.Add(new JObject
                    {
                        {"Name", balance.Name},
                        {"Balance", balance.Balance},
                    });
            }
            return j_customerbalance;
        }
        [HttpPost]
        public JArray UserOrderMeal(Slack slack)
        {
            var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]);
            var j_ordermeal = new JArray();
            Customer customer = _lunchRepository.GetCustomerBySalckId(slack.slackId);
            if (customer == null)
            {
                j_ordermeal.Add(new JObject
                {
                    {"Status",false },
                    {"Message","你尚未設定slackId" }
                });
            }
            else
            {
                var balance = _lunchRepository.RecentBalance(customer.Name);
                if (balance < -1000)
                {
                    j_ordermeal.Add(new JObject
                    {
                        {"Status",false },
                        {"Message","你已負債1000元，請先到OD儲值再來點餐!" }
                    });
                }
                else
                {
                    bool IsPushed = connection.Query<bool>("select IsPushed from Restaurant where RestaurantId = @rid", new { rid = slack.restaurantId }).FirstOrDefault();
                    if (!IsPushed)
                    {
                        j_ordermeal.Add(new JObject
                        {
                            {"Status",false },
                            {"Message","此餐廳沒有開放點餐" }
                        });
                    }
                    else
                    {
                        bool IsClose = connection.Query<bool>("SELECT [IsClose] FROM [Restaurant] WHERE [RestaurantId]=@O_rid",
                            new { O_rid = slack.restaurantId }).FirstOrDefault();
                        if (IsClose)
                        {
                            j_ordermeal.Add(new JObject
                            {
                                {"Status",false },
                                {"Message","此餐廳已經收單囉" }
                            });
                        }
                        else
                        {
                            connection.Execute(
                                "INSERT INTO [Order]([Meal],[Cost],[Time],[CustomerId],[Settle],[RestaurantId]) values(@meal,@costs,@time,@C_Id,@O_settle,@O_rid)",
                                new
                                {
                                    meal = slack.meal,
                                    costs = slack.price,
                                    time = DateTime.Now,
                                    C_Id = customer.CustomerId,
                                    O_settle = false,
                                    O_rid = slack.restaurantId
                                });
                            j_ordermeal.Add(new JObject
                            {
                                {"Status",true },
                                {"Message","點餐成功" }
                            });
                        }
                    }
                }                         
            }
            return j_ordermeal;
        }
        [HttpPost]
        public JArray GetUserOrder(Slack slack)
        {
            var j_ordermeal = new JArray();
            Customer customer = _lunchRepository.GetCustomerBySalckId(slack.slackId);
            if (customer == null)
            {
                j_ordermeal.Add(new JObject
                {
                    {"Status",false },
                    {"Message","你尚未設定slackId" },
                    {"RestaurantId",null },
                    {"RestaurantName",null },
                    {"Meal",null },
                    {"Cost",null }
                });
            }
            else
            {
                var userorder = _lunchRepository.GetUserOrder(customer.Name);
                if (userorder.Count()==0)
                {
                    j_ordermeal.Add(new JObject
                    {
                        {"Status",false },
                        {"Message","你沒有點任何餐點" },
                        {"RestaurantId",null },
                        {"RestaurantName",null },
                        {"Meal",null },
                        {"Cost",null }
                    });
                }
                else
                {
                    foreach (var order in userorder)
                    {
                        var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]);
                        string RestaurantName = connection.Query<string>("select Name from Restaurant where RestaurantId = @rid", new { rid = order.RestaurantId }).FirstOrDefault().ToString();
                        j_ordermeal.Add(new JObject
                        {
                            {"Status",true },
                            {"Message","slackId設定成功" },
                            {"RestaurantId",order.RestaurantId },
                            {"RestaurantName",RestaurantName },
                            {"Meal",order.Meal },
                            {"Cost",order.Cost }
                        });
                    }
                }
            }
            return j_ordermeal;
        }
        [HttpPost]
        public string DeleteUserMeal(Slack slack)
        {
            Customer customer = _lunchRepository.GetCustomerBySalckId(slack.slackId);
            if (customer == null)
            {
                return "你尚未設定slackId";
            }
            else
            {
                var userorder = _lunchRepository.GetUserOrder(customer.Name);
                if (userorder == null)
                {
                    return "你沒有點任何餐點";
                }
                else
                {
                    foreach (var order in userorder)
                    {
                        _lunchRepository.DeleteOrder(order.OrderId);
                    }
                }
                return "刪除所有餐點成功";
            }
        }
        // GET: api/SlackApi

        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/SlackApi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SlackApi
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/SlackApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/SlackApi/5
        public void Delete(int id)
        {
        }
    }

    public class Slack
    {
        public string slackId { get; set; }
        public int restaurantId { get; set; }
        public string meal { get; set; }
        public int price { get; set; }
    }
}
