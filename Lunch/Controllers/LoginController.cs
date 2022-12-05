using Lunch.Service;
using System.Web.Mvc;

namespace Lunch.Controllers
{
    public class LoginController : BaseController
    {
        private EncryptProcessor encryptor = new EncryptProcessor();

        public JsonResult Test()
        {
            return Json(encryptor.getEncryptedStr("password"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Saltpassword()
        {
            var customer = LunchRepository.GetAllCustomer();
            foreach (var cus in customer)
            {
                LunchRepository.SaltCustomerPassword(encryptor.getEncryptedStr(cus.Password),cus.CustomerId);
            }
            return Json("Sucess", JsonRequestBehavior.AllowGet);
        }
        // GET: Login
        public ActionResult Index(string alreadyRegist)
        {
            if (alreadyRegist != null)
            {
                if (alreadyRegist == "yes")
                {
                    return View(new RegistStatus {registStatus = "HadRegistBefore"});
                }
            }

            return View(new RegistStatus());
        }

        public class RegistStatus
        {
            public string registStatus { get; set; }
        }

        [HttpPost]
        public ActionResult Index(string username, string password)
        {
            password = encryptor.getEncryptedStr(password);
            var login = LunchRepository.GetLoginInformation(username, password); //與資料庫比對是否有view輸入的帳密
            if (login == null) //以防後面的login.admin判斷出錯(booling不可以是null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (login.Permission == 3)
            {
                ViewBag.pendingcheck = "True";
                return View(new RegistStatus());
            }
            Session["User"] = login;
            //利用session把資料傳出去(所有的Session["User"] 由此開始?)，User可隨便命名，已利用session做登入管制，所以尚未登入的人不可以進入點餐頁面

            if (login.Admin) //login!=null 代表資料庫有符合的一組帳號密碼
            {
                return RedirectToAction("Index", "Restaurant"); //進入主頁  
            }

            return RedirectToAction("Index", "ClientOrder",
                new {LGstatusname = login.Name, LGstatusCustomerId = login.CustomerId});
        }
    }
}