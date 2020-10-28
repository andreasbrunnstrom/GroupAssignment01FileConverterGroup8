using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FileConverter.Models
{
    [Serializable()]
    public class ProductData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime? AvailableUntil { get; set; }
        public UnitPrice UnitPrice = new UnitPrice();
        public List<string> AvailableInMarkets { get; set; } = new List<string>();
        [XmlArrayItem("PropertyData")]
        public List<Property> Properties { get; set; } = new List<Property>();
        public List<string> Sizes { get; set; } = new List<string>();

    }

}
