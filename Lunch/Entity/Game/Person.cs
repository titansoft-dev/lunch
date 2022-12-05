using System.Xml.Serialization;

namespace Lunch.Entity.Game
{
    public class Person
    {
        [XmlIgnore]
        public int Rank { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("score")]
        public int Score { get; set; }
    }
}