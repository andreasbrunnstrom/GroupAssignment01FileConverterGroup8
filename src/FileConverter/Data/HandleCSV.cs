using System;
using System.Collections.Generic;
using System.Globalization;
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
            string[] rows = GetFileRows(fileStream);
            List<ProductData> events = new List<ProductData>();

            foreach (var row in rows)
            {
                try
                {
                    ProductData product = CreateEvent(row);
                    events.Add(product);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return events;
        }

        private string[] GetFileRows(Stream fileStream)
        {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    List<string> rows = new List<string>();
                    string currentRow;
                    while ((currentRow = reader.ReadLine()) != null)
                    {
                        rows.Add(currentRow);
                    }
                    return rows.ToArray();
                }           
        }

        private ProductData CreateEvent(string row)
        {
            string[] columns = row.Split(";");

            ProductData productData = new ProductData
            {
                Id = columns[0],
                Name = columns[1]                
        };
            if (!string.IsNullOrEmpty(columns[2]))
            {
                productData.DisplayName = columns[2];
            }
            if (!string.IsNullOrEmpty(columns[3]))
            {
                productData.AvailableFrom = DateTime.ParseExact(columns[3], "yyyy-MM-dd:00:00:00", CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(columns[4]))
            {
                productData.AvailableUntil = DateTime.ParseExact(columns[4], "yyyy-MM-dd:00:00:00", CultureInfo.InvariantCulture);
            }

            PropertyData property;
            UnitPrice unitPrice = new UnitPrice();           
            if (!string.IsNullOrEmpty(columns[5]))
            {
                unitPrice.Amount = int.Parse(columns[5]);                
            }
            if (!string.IsNullOrEmpty(columns[6]))
            {
                unitPrice.Currency = columns[6];
            }
            if (!string.IsNullOrEmpty(columns[7]))
            {
                productData.AvailableInMarkets.Add(columns[7]); // if NO SE DNK
            }
            if (!string.IsNullOrEmpty(columns[8]))
            {
                property = CreateStringProperty("Description", columns[8]);
                productData.Properties.Add(property);
            }
            if (!string.IsNullOrEmpty(columns[9]))
            {
                property = CreateStringProperty("DelieveryNote", columns[9]);
                productData.Properties.Add(property);
            }
            if (!string.IsNullOrEmpty(columns[10]))
            {
                property = CreateIntProperty("DeliveryFromDays", columns[10]);
                productData.Properties.Add(property);
            }
            if (!string.IsNullOrEmpty(columns[11]))
            {
                property = CreateIntProperty("DeliveryToDays", columns[11]);
                productData.Properties.Add(property);
            }
            if (!string.IsNullOrEmpty(columns[12]))
            {
                property = CreateBoolProperty("ProductSoldOut", columns[12].ToString());
            }
            if(!string.IsNullOrEmpty(columns[13]))
            {
                productData.Sizes.Add(columns[13]);  // Several options                          
            }
            return productData;
        }

        private PropertyData CreateIntProperty(string name, string data)
        {
            return new PropertyData
            {
                Name = name,
                Value = int.Parse(data)
            };
        }

        private PropertyData CreateStringProperty(string name, string data)
        {
            return new PropertyData
            {
                Name = name,
                Value = data
            };
        }

        private PropertyData CreateBoolProperty(string name, string data)
        {
            return new PropertyData
            {
                Name = name,
                Value = bool.Parse(data)
            };
        }
        //public object Read(Stream fileStream)
        //{
        //    List<ProductData> products = new List<ProductData>();
        //    using (StreamReader streamReader = new StreamReader(fileStream))
        //    {               
        //        while (!streamReader.EndOfStream)
        //        {
        //            ProductData productData = new ProductData();
        //            string[] columns = new string[14];
        //            columns = streamReader.ReadLine().Split(";");



        //            productData.Id = columns[0];
        //            productData.Name = columns[1];
        //            if (!string.IsNullOrEmpty(columns[2]))
        //                productData.DisplayName = columns[2];

        //            productData.AvailableFrom = Convert.ToDateTime(columns[3]);

        //            if (columns[4] != "")
        //            {
        //                productData.AvailableUntil = Convert.ToDateTime(columns[4]);
        //            }

        //            productData.UnitPrice.Amount = Convert.ToDecimal(columns[5]);
        //            productData.UnitPrice.Currency = columns[6];

        //            List<string> markets = columns[7].Split(',').ToList();
        //            productData.AvailableInMarkets.AddRange(markets);

        //            List<string> sizes = columns[8].Split(',').ToList();
        //            productData.Sizes.AddRange(sizes);

        //            productData.Properties.Add(new PropertyData { Name = "Description", Value = columns[9] });
        //            if(columns.Length == 10) { productData.Properties.Add(new PropertyData { Name = "DelieveryNote", Value = columns[10] }); }              
        //            if(columns.Length == 11) { productData.Properties.Add(new PropertyData { Name = "DeliveryFromDays", Value = columns[11] }); }
        //            if(columns.Length == 12) { productData.Properties.Add(new PropertyData { Name = "DeliveryToDays", Value = columns[12] }); }                 
        //            if(columns.Length == 13) { productData.Properties.Add(new PropertyData { Name = "ProductSoldOut", Value = columns[13] }); }
        //            products.Add(productData);
        //        }               
        //    }
        //    return products;
        //}

        public void Write(Stream stream, object data)
        {
            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
                StringBuilder csv = new StringBuilder();
                List<ProductData> products = (List<ProductData>)data;

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
                streamWriter.Write(csv.ToString().ToArray());
            }
        }
    }
}
