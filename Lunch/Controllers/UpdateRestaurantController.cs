using System.Web;
using System.Web.Mvc;
//using Lunch.Repository;
using System.IO;

namespace Lunch.Controllers
{
    public class UpdateRestaurantController : AdminBaseController
    {
        // GET: UpdateRestaurant
        public ActionResult Index(int? restaurantId) //透過ResId拿取欲修改的餐廳資訊show到update的UI
        {
            var restaurant = LunchRepository.GetRestaurantById(restaurantId.Value);
            if (restaurant.Category == 1)
            {
                ViewBag.category = "category1";
            }
            else if (restaurant.Category == 2)
            {
                ViewBag.category = "category2";
            }
            else if (restaurant.Category == 3)
            {
                ViewBag.category = "category3";
            }
            else if (restaurant.Category == 4)
            {
                ViewBag.category = "category4";
            }
            else if (restaurant.Category == 5)
            {
                ViewBag.category = "category5";
            }
            else if (restaurant.Category == 6)
            {
                ViewBag.category = "category6";
            }
            return View(restaurant);
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase img, int rid, string name, string tel, string url, string comment,
            int category)
        {
            if (img != null)
            {
                if (img.ContentLength > 0)
                {
                    string[] urlStr = url.Split('\\');
                    var serverpath = Server.MapPath("~/images/Menus");
                    var oldurlpath = Path.Combine(serverpath, urlStr[urlStr.Length - 1]);

                    System.IO.File.Delete(oldurlpath);
                    var restaurant = LunchRepository.GetRestaurantById(rid);
                    var fileName = Path.GetFileName(img.FileName);
                    var path = Path.Combine(Server.MapPath("~/images/Menus"), fileName);
                    img.SaveAs(path);
                    url = Path.Combine("/images/Menus", fileName);
                }
            }
            //comment = comment.Replace("\r\n", "<br />");
            LunchRepository.UpdateRestaurant(name, tel, url, comment, rid, category);
            return RedirectToAction("Index", "Restaurant",new { restaurantId =rid});
        }
    }
}