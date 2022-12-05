using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Serialization;

using Lunch.Entity.Game;
using Lunch.Entity;

namespace Lunch.Controllers
{
    public class GamesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MiniGame()
        {
            var customer = Session["User"] as Customer; 
            ViewBag.ShowGame = ConfigurationManager.AppSettings["showGame"];
            ViewBag.UserName = GetUserName();

            return PartialView("_MiniGame");
        }

        public ActionResult Ranks(Person model)
        {
            var customer = Session["User"] as Customer;           
            Record result = null;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\record.xml");
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Record));
                result = serializer.Deserialize(stream) as Record;
                if (result == null)
                {
                    result = new Record();
                }

                model.Name = GetUserName();
                result.Persons.Add(model);
                result.Persons = result.Persons.OrderByDescending(p => p.Score).ToList();
                var scoreGroup = result.Persons.ToLookup(p => p.Score);

                int i = 1;
                foreach (var item in scoreGroup)
                {
                    if (i > 10)
                    {
                        break;
                    }

                    var persons = item.ToList();
                    persons.ForEach(p => p.Rank = i);
                    i += persons.Count;
                }

                result.Persons.RemoveAll(p => p.Rank == 0 || result.Persons.FindIndex(tmp => tmp == p) > 9);
                stream.SetLength(0);
                stream.Seek(0, SeekOrigin.Begin);
                serializer.Serialize(stream, result);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string GetUserName()
        {
            var customer = Session["User"] as Customer;
            return customer == null ? "Guest" : customer.Name;
        }
    }
}