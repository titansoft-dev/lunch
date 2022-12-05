using System.Collections.Generic;

namespace Lunch.Entity
{
    public class LoginResponse
    {
        public Customer Customer;
        public List<OrderInfo> Orders;
    }
}