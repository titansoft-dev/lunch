using System;

namespace Lunch.Entity
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public DateTime RegistTime { get; set; }
        public int Balance { get; set; }
        public bool Login { get; set; }
        public bool Admin { get; set; }
        public int Permission { get; set; }
        public string SlackId { get; set; }
    }
}