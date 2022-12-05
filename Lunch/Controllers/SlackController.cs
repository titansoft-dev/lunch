using Lunch.Entity;
using Lunch.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lunch.Controllers
{
    public class SlackController : UserBaseController
    {
        // GET: Slack
        public ActionResult Index()
        {
            var user = Session["User"] as Customer;
            var customer = new LunchRepository().GetUserSlackId(user.Name);
            var cust = new SlackViewModel();
            cust.customer = customer;
            return View(cust);
        }

        [HttpPost]
        public ActionResult Index(string slackId)
        {
            var user = Session["User"] as Customer;
            new LunchRepository().UpdateSlackId(slackId, user.Name);
            TempData["IsSuccess"] = "Slack Id更新成功";
            return RedirectToAction("Index");
        }

        public class SlackViewModel
        {
            public IEnumerable<Customer> customer { get; set; }
        }
    }
}