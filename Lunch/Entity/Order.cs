using System;


namespace Lunch.Entity
{
    public class Order
    {
        public int RestaurantId { get; set; }
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public string Meal { get; set; }
        public int Cost { get; set; }
        public DateTime Time { get; set; }
        public int Settle { get; set; }
    }
}