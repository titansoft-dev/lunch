using System;

namespace Lunch.Entity
{
    public class Summary
    {
        public int SummaryId { get; set; }
        public int RestaurantId { get; set; }
        public int CustomerId { get; set; }
        public string Meal { get; set; }
        public int Cost { get; set; }
        public DateTime Time { get; set; }
        public int Balance { get; set; }
        public string Name { get; set; }
        public int NameNum { get; set; }
    }
}