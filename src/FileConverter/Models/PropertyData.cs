using System;

namespace FileConverter.Models
{
    [Serializable()]
    public class PropertyData
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
