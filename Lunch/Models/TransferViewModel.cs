using System.Collections.Generic;
using System.Linq;
using Lunch.Entity;
using Lunch.Repository;

namespace Lunch.Models
{
    public class TransferViewModel
    {
        public List<OrderInfo> RequestToList { get; set; }

        public List<OrderInfo> requestFromList { get; set; }

        public List<Customer> customerList { get; set; }

        public TransferViewModel(int custId)
        {
            var lunchRepository = new LunchRepository();
            var transferList = lunchRepository.GetTransferList().ToList();
            RequestToList = transferList.Where(x => x.CustomerId == custId).ToList();
            requestFromList = transferList.Where(x => x.RestaurantId == custId).ToList();
        }

        public void GetAllCustomerList()
        {
            var lunchRepository = new LunchRepository();
            customerList = lunchRepository.GetName().ToList();
        }
    }
}