using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FileConverter.Models;

namespace FileConverter.Data
{
    class HandleCSV : IFileHandler
    {
        public string Extension { get; } = ".csv";

        public object Read(Stream fileStream)
        {
            List<ProductData> products = new List<ProductData>();
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                while (!streamReader.EndOfStream)
                {
                    string[] columns;
                    columns = streamReader.ReadLine().Split(";");

                    ProductData productData = new ProductData();

                    productData.Id = columns[0];
                    productData.Name = columns[1];
                    productData.DisplayName = columns[2];
                    productData.AvailableFrom = Convert.ToDateTime(columns[3]);

                    if (columns[4] != "")
                    {
                        productData.AvailableUntil = Convert.ToDateTime(columns[4]);
                    }

                    productData.UnitPrice.Amount = Convert.ToDecimal(columns[5]);

                    List<string> markets = columns[6].Split(',').ToList();
                    productData.AvailableInMarkets.AddRange(markets);

                    List<string> sizes = columns[7].Split(',').ToList();
                    productData.Sizes.AddRange(sizes);

                    productData.Properties.Add(new PropertyData { Name = "Description", Value = columns[8] });
                    productData.Properties.Add(new PropertyData { Name = "DelieveryNote", Value = columns[9] });
                    productData.Properties.Add(new PropertyData { Name = "DelieveryFromDays", Value = columns[10] });
                    productData.Properties.Add(new PropertyData { Name = "DelieveryToDays", Value = columns[11] });
                    productData.Properties.Add(new PropertyData { Name = "ProductSoldOut", Value = columns[12] });

                    products.Add(productData);
                }
            }
            return products.ToArray();
        }

        public void Write(Stream stream, object data)
        {
            using (StreamWriter steamWriter = new StreamWriter(stream))
            {
                StringBuilder csv = new StringBuilder();
                ProductData[] products = (ProductData[])data;

                foreach (var product in products)
                {
                    string markets = string.Join(',', product.AvailableInMarkets);
                    string sizes = string.Join(',', product.Sizes);
                    List<string> props = new List<string>();
                    foreach (PropertyData prop in product.Properties)
                    {
                        props.Add(prop.Value.ToString());
                    }
                    string properties = string.Join(';', props);
                    var line = $"{product.Id};{product.Name};{product.DisplayName};" +
                        $"{product.AvailableFrom};{product.AvailableUntil};" +
                        $"{product.UnitPrice.Amount};{markets};" +
                        $"{sizes};{properties}";

                    csv.AppendLine(line);
                }
                steamWriter.Write(csv.ToString());
            }
        }
    }
}
