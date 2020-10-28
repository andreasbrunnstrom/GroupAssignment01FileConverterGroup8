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
        public UnitPrice UnitPrice { get; set; }
        public List<string> AvailableInMarkets { get; set; }
        public List<PropertyData> Properties { get; set; }
        public List<string> Sizes { get; set; }
    }
}
