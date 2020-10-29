using System;
using System.Collections.Generic;

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
        public UnitPrice UnitPrice { get; set; } = new UnitPrice();
        public List<string> AvailableInMarkets { get; set; } = new List<string>();
        public List<PropertyData> Properties { get; set; } = new List<PropertyData>();
        public List<string> Sizes { get; set; } = new List<string>();
    }
}
