using System.Collections.Generic;
using Lunch.Entity;

namespace Lunch.Models
{
    public class ClientOrderModelView
    {
        public List<Restaurant> restaurants { get; set; }
        public Restaurant currentRestaurant { get; set; }
        public IEnumerable<OrderInfo> allOrder { get; set; }
        public string user { get; set; }
        public bool Admin { get; set; }
        public IEnumerable<Customer> allCustomer { get; set; }
        public List<Recommend> recommends { get; set; }
    }
}