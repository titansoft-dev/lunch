using System.Web;
using System.Web.Mvc;
using System.IO;
using Lunch.Helper;

//using Lunch.Repository;

namespace Lunch.Controllers
{
    public class InsertRestaurantController : AdminBaseController
    {
        // GET: InsertRestaurant
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase img, string name, string tel, int category)
        {
            try
            {
                if (img.ContentLength > 0 & name != null & tel != null)
                {
                    var fileName = Path.GetFileName(img.FileName);
                    var path = Path.Combine(Server.MapPath("~/images/Menus"), fileName);
                    img.SaveAs(path);

                    var n = Path.GetFileName(img.FileName);
                    string url = "/images/Menus\\" + n;
                    LunchRepository.InsertRestaurant(name, tel, url, category);

                    ViewBag.isSuccess = "true";
                }
                else
                {
                    ViewBag.isSuccess = "false";
                }
            }
            catch (System.Exception e)
            {
                LogHelper.Log("Insert Error: " + e.ToString());
            }
            return View();
        }
    }
}