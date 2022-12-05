using Lunch.Entity;
//using Lunch.Repository;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Lunch.Controllers
{
    public class IndividualCostController : BaseController
    {
        // GET: IndividualCost
        public ActionResult Index()
        {
            var individualcost = LunchRepository.IndividualCost(); //從IndividualCost()的SP中拿到每個人當日點餐的金額加總
            //var notordername = LunchRepository.GetNotOrder();//從GetNotOrder()的SP中拿到尚未點餐的人的名字，塞給NotOrderName


            return View(new IndividualViewModel {IndCost = individualcost /*, NotOrderName = notordername*/});
                //把兩個由SP取出來的IEnuberable分別由IndividualViewModel傳回view
        }
    }

    public class IndividualViewModel
    {
        public IEnumerable<IndCost> IndCost { get; set; } //前IndCost為Model名稱
        public IEnumerable<Customer> NotOrderName { get; set; }
    }
}