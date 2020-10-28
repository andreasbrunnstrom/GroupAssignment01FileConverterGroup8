using System;

namespace FileConverter.Models
{
    [Serializable()]
    public class UnitPrice
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
