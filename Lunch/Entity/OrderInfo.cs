using System;

namespace Lunch.Entity
{
    public class OrderInfo
    {
        public string Name { get; set; }
        public string Meal { get; set; }
        public int Cost { get; set; }
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public int Number { get; set; }
        public bool Settle { get; set; }
        public int IndSum { get; set; }
        public int NameNum { get; set; }
        public DateTime Time { get; set; }
        public string OrderStatus { get; set; }
    }
}