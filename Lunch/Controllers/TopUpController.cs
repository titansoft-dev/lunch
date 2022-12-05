using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Data.SqlClient;
using Lunch.Entity;
using Dapper;
using Newtonsoft.Json;


namespace Lunch.Controllers
{
    public class TopUpController : DelegatorBaseController
    {
        // GET: TopUp
        public ActionResult Index()
        {
            var customername = LunchRepository.GetCustomerName2(); //抓summary中的人名以最終金額
            var customerbalance = LunchRepository.GetCustomerBalance2(200);
            var cust = new TopUpViewModel();
            cust.custbalance = customerbalance;
            cust.custname = customername;
            return View(cust);
        }

        public class TopUpViewModel
        {
            public IEnumerable<Customer> custname { get; set; }
            public IEnumerable<Customer> custbalance { get; set; }
        }

        [HttpPost]
        public ActionResult Index(TopUp Tform)
        {
            var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]); //
            if (Tform.TopUpNumber == 0)
            {
                var balance =
                    connection.Query<int>(
                        "SELECT [Balance] FROM [Summary] WHERE [CustomerId]=@customerId ORDER BY [SummaryId] desc",
                        new { customerId = Tform.str2 }).First(); //抓特定人的balance出來
                connection.Dispose(); //
                return Json(balance);
            }
            else //儲值的功能
            {
                var resentbalance =
                    connection.Query<int>(
                        "SELECT [Balance] FROM [Summary] WHERE [CustomerId]=@customerId ORDER BY [SummaryId] desc",
                        new { customerId = Tform.str2 }).FirstOrDefault();
                connection.Execute(
                    "INSERT INTO [Summary]([CustomerId],[Balance],[Name],[Time],[Meal],[Cost]) values(@C_custId,@C_balance,@C_name,@S_time,@S_Meal,@S_cost)",
                    new
                    {
                        C_custId = Tform.str2,
                        C_balance = Tform.TopUpNumber + resentbalance,
                        C_name = Tform.customername,
                        S_time = DateTime.Now,
                        S_Meal = Tform.detail,
                        S_cost = Tform.TopUpNumber
                    });
                connection.Dispose(); //
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Index2(TopUp T)
        {
            var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]);
            var customerbalance = LunchRepository.GetCustomerBalance2(T.limit);
            var jsonBalance = JsonConvert.SerializeObject(customerbalance);
            return Json(jsonBalance);
        }
    }
}