using System;
using System.Security.Cryptography;
using System.Web.Mvc;
using Lunch.Entity;
using Lunch.Service;

namespace Lunch.Controllers
{
    public class MemberController : DelegatorBaseController
    {
        private EncryptProcessor encryptor = new EncryptProcessor();
        // GET: Member
        public ActionResult Index()
        {
            var AllMember = LunchRepository.GetName2();
            return View(AllMember);
        }

        [HttpPost]
        public ActionResult Index(Customer c)
        {
            LunchRepository.UpdatePermission(c.Permission, c.CustomerId);
            if (c.Permission == 1 || c.Permission == 2)
            {
                LunchRepository.UpdateAdmin(c.CustomerId, true);
            }
            else
            {
                LunchRepository.UpdateAdmin(c.CustomerId, false);
                string registername = LunchRepository.GetNameById(c.CustomerId);
                //var custname = LunchRepository.GetName2();//想要通過註冊時再加入summary table 失敗
                //foreach (var cust in custname) {
                //    if (registername == cust.Name)
                //    {
                //        if (c.Permission == 3)//Permission==0 一般使用者，Permission==1 Admin，Permission==2 代理人，Permission==3 Pending，Permission==4 已被刪除的人
                //        {
                //            LunchRepository.AddRegistDataToSummary(registername, c.CustomerId);
                //        }
                //    }

                //}
            }

            return null;
        }

        [HttpPost]
        public ActionResult Delete(Customer c)
        {
            LunchRepository.DeleteCustomer(c.CustomerId);
            return null;
        }
        [HttpPost]
        public ActionResult ResetPW(Customer c)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            char[] chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            int length = RNG.Next(8, 8);
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[RNG.Next(chars.Length - 1)]);
            }
            string YourPassword = sb.ToString();
            string encryptedPwd = encryptor.getEncryptedStr(YourPassword);
            LunchRepository.ResetUserPassword(c.CustomerId, encryptedPwd);
            return Json(YourPassword);
        }
    }
    public static class RNG
    {
        private static RNGCryptoServiceProvider rngp = new RNGCryptoServiceProvider();
        private static byte[] rb = new byte[4];

        /// <summary>
        /// 產生一個非負數的亂數
        /// </summary>
        public static int Next()
        {
            rngp.GetBytes(rb);
            int value = BitConverter.ToInt32(rb, 0);
            if (value < 0) value = -value;
            return value;
        }

        /// <summary>
        /// 產生一個非負數且最大值 max 以下的亂數
        /// </summary>
        /// <param name="max">最大值</param>
        public static int Next(int max)
        {
            rngp.GetBytes(rb);
            int value = BitConverter.ToInt32(rb, 0);
            value = value % (max + 1);
            if (value < 0) value = -value;
            return value;
        }

        /// <summary>
        /// 產生一個非負數且最小值在 min 以上最大值在 max 以下的亂數
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public static int Next(int min, int max)
        {
            int value = Next(max - min) + min;
            return value;
        }
    }
}