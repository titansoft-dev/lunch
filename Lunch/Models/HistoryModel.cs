using System.Collections.Generic;
using Lunch.Entity;

namespace Lunch.Models
{
    public class HistoryModel
    {
        public IEnumerable<Summary> balance { get; set; }
        public IEnumerable<Customer> customer { get; set; }
    }
}