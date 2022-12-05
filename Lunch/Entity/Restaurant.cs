using System;

namespace Lunch.Entity
{
    public class Restaurant
    {
        public string Name { get; set; }
        public int RestaurantId { get; set; }
        public int Category { get; set; }
        public string Telephone { get; set; }
        public string MenuUrl { get; set; }
        public bool IsPushed { get; set; }
        public bool IsClose { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string Comment { get; set; }
    }
}