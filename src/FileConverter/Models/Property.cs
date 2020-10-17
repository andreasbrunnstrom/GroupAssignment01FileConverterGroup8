using System;
using System.Collections.Generic;
using System.Text;

namespace FileConverter.Models
{
    [Serializable()]
    public class Property
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
