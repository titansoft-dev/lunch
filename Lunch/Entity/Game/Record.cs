using System.Collections.Generic;
using System.Xml.Serialization;

namespace Lunch.Entity.Game
{
    [XmlRoot("record")]
    public class Record
    {
        public Record()
        {
            Persons = new List<Person>();
        }

        [XmlArray("persons")]
        [XmlArrayItem("person")]
        public List<Person> Persons { get; set; }
    }
}