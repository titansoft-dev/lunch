//using Lunch.Repository;
using Dapper;
using Lunch.Service;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Lunch.Controllers
{
    public class RegisterController : BaseController
    {
        // GET: Register
        private EncryptProcessor encryptor = new EncryptProcessor();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string registername, string registerpassword)
        {
            //TODO: Update customer table and do checking
            var connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]); //
            var CustomerName = LunchRepository.GetName();
            string regname = registername.ToLower();
            //if (registername.ToLower().Contains(registername))
            if (registername != null)
            {
                foreach (var name in CustomerName)
                {
                    if (name.Name.ToLower().Contains(regname))
                    {
                        connection.Dispose(); //
                        return RedirectToAction("Index", "Login", new {alreadyRegist = "yes"});
                    }
                }
                //var Password = FormsAuthentication.HashPasswordForStoringInConfigFile(registerpassword, "SHA1");//利用SHA1加密

                registerpassword = encryptor.getEncryptedStr(registerpassword);
                connection.Execute(
                    "INSERT INTO [Customer]([name],[password],[RegistTime],[Permission],[Admin]) values(@name,@password,@RTime,@per,@admin)",
                    new {name = registername, password = registerpassword, RTime = DateTime.Now, per = 3, admin = false});
                    //申請帳號時利用getdate()一起加入時間
                int Cid =
                    connection.Query<int>("SELECT [CustomerId] FROM [Customer] WHERE [Name]=@C_name ",
                        new {C_name = registername}).FirstOrDefault();
                LunchRepository.AddRegistDataToSummary(registername, Cid); //註冊時加入把資料加入summary table
                //password = FormsAuthentication.HashPasswordForStoringInConfigFile(registerpassword, "SHA1");//利用SHA1加密
                connection.Dispose(); //
                return RedirectToAction("Index", "Login");
            }
            /*if (CustomerName.Select(x => x.Name).ToList().Contains(registername))//Contains換成Equals("Steven"))可以偵測""內的東西 //把customer的name轉成list,在去看list裡面有沒有包含註冊的registername,有一樣的就不給註冊
                {
                    //Response.Write("<script>alert('這是一個 Javascript Alert');</script>"); //return alert, but not working
                    connection.Dispose();//
                    return RedirectToAction("Index", "Login", new {alreadyRegist = "yes" });  //若已經有一樣的帳號密碼，則傳yes回loginController
                }*/
            connection.Dispose(); //
            return RedirectToAction("Index");
        }
    }
}