using System;
using System.Web.Mvc;
//using Lunch.Repository;
using Lunch.Entity;
using Lunch.Models;

namespace Lunch.Controllers
{
    public class BalanceController : UserBaseController
    {
        // GET: Balance
        public ActionResult Index()
        {
            var user = Session["User"] as Customer;
            var admin = LunchUser.Admin.ToString();
            ViewBag.Admin = admin;
            DateTime today = DateTime.Today;
            DateTime date1, date2;
            date1 = date2 = today;
            ViewBag.date1 = date1.ToString("yyyy-MM-dd");
            ViewBag.date2 = date2.ToString("yyyy-MM-dd");
            ViewBag.date = "Today";
            ViewBag.target = -1;
            date2 = date2.AddDays(+1);
            if (user.Admin == true)
            {
                var balance = LunchRepository.GetBalance2(date1, date2);
                var customer = LunchRepository.GetCustomerName();
                var history = new HistoryModel();
                history.balance = balance;
                history.customer = customer;
                return View(history);
            }
            else
            {
                var balance = LunchRepository.GetIndBalance(date1, date2, user.CustomerId);
                var customer = LunchRepository.GetCustomerName();
                var history = new HistoryModel();
                history.balance = balance;
                history.customer = customer;
                return View(history);
            }
        }

        [HttpPost]
        public ActionResult Index(DateTime date1, DateTime date2, int? target)
        {
            var user = Session["User"] as Customer;
            var admin = LunchUser.Admin.ToString();
            ViewBag.Admin = admin;
            ViewBag.date1 = date1.ToString("yyyy-MM-dd");
            ViewBag.date2 = date2.ToString("yyyy-MM-dd");
            ViewBag.date = "From  " + ViewBag.date1 + "  to  " + ViewBag.date2 + " ";
            ViewBag.target = target;
            date2 = date2.AddDays(+1);
            if (user.Admin == true)
            {
                if (target == -1)
                {
                    var balance = LunchRepository.GetBalance2(date1, date2);
                    var customer = LunchRepository.GetCustomerName();
                    var history = new HistoryModel();
                    history.balance = balance;
                    history.customer = customer;
                    return View(history);
                }
                else
                {
                    var balance = LunchRepository.GetIndBalance(date1, date2, target.Value);
                    var customer = LunchRepository.GetCustomerName();
                    var history = new HistoryModel();
                    history.balance = balance;
                    history.customer = customer;
                    return View(history);
                }
            }
            else
            {
                var balance = LunchRepository.GetIndBalance(date1, date2, user.CustomerId);
                var customer = LunchRepository.GetCustomerName();
                var history = new HistoryModel();
                history.balance = balance;
                history.customer = customer;
                return View(history);
            }
        }
    }
}