using System;
using System.Collections.Generic;
using System.Text;

namespace FileConverter.Models
{
    [Serializable()]
    public class UnitPrice
    {
        public double Amount { get; set; }
        public string Currency { get; set; }
    }
}
