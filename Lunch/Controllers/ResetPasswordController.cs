using Lunch.Entity;
using Lunch.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Lunch.Controllers
{
    public class ResetPasswordController : BaseController
    {
        private EncryptProcessor encryptor = new EncryptProcessor();
        // GET: ResetPassword
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UpdatePassword(string password)
        {
            var user = Session["User"] as Customer;
            if (password != "")
            {
                string encryptedPwd = encryptor.getEncryptedStr(password);
                LunchRepository.ResetUserPassword(user.CustomerId, encryptedPwd);
            }
            return null;
        }
    }
}