using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lunch.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lunch.Controllers
{
    public class ApiController : UserBaseController
    {
        //
        // GET: /Api/
        public JsonResult GetCustomerInfo()
        {
            var user = Session["User"] as Customer;
            var allCustList = LunchRepository.GetName().ToList();
            var custList = allCustList.Where(x => x.CustomerId != user.CustomerId).Select(cust => new Dictionary<string, string> { { "id", cust.CustomerId.ToString() }, { "name", cust.Name } }).ToList();
            return Json(custList);
        }
    }
}