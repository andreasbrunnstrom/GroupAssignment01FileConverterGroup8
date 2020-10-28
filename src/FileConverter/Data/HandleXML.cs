using FileConverter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace FileConverter.Data
{
    class HandleXML : IFileHandler
    {
        public string Extension { get; } = ".xml";

        public object Read(Stream fileStream)
        {
            List<ProductData> products = new List<ProductData>();
            try
            {               
                var serializer = new XmlSerializer(typeof(List<ProductData>));
                products = serializer.Deserialize(fileStream) as List<ProductData>;
            }
            catch
            {
                throw;
            }
            return products;
        }

        public void Write(Stream stream, object data)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ProductData>));
            xmlSerializer.Serialize(stream, data);
        }
    }
}
