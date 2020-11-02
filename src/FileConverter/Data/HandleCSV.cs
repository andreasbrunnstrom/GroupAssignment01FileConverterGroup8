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
                    if(columns.Length == 10) { productData.Properties.Add(new PropertyData { Name = "DelieveryNote", Value = columns[9] }); }              
                    if(columns.Length == 11) { productData.Properties.Add(new PropertyData { Name = "DelieveryFromDays", Value = columns[10] }); }
                    if(columns.Length == 12) { productData.Properties.Add(new PropertyData { Name = "DelieveryToDays", Value = columns[11] }); }                 
                    if(columns.Length == 13) { productData.Properties.Add(new PropertyData { Name = "ProductSoldOut", Value = columns[12] }); }
                    
                    products.Add(productData);
                }
            }
            return products.ToArray();
        }

        public void Write(Stream stream, object data)
        {
            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
                StringBuilder stringBuilder = new StringBuilder();
                List<ProductData> products = new List<ProductData>();

                foreach (var productData in products)
                {
                    string markets = string.Join(',', productData.AvailableInMarkets);
                    string sizes = string.Join(',', productData.Sizes);
                    List<string> props = new List<string>();
                    foreach (PropertyData prop in productData.Properties)
                    {
                        props.Add(prop.Value.ToString());
                    }
                    string properties = string.Join(';', props);
                    var line = $"{productData.Id};{productData.Name};{productData.DisplayName};" +
                        $"{productData.AvailableFrom};{productData.AvailableUntil};" +
                        $"{productData.UnitPrice.Amount};{markets};" +
                        $"{sizes};{properties}";

                    stringBuilder.AppendLine(line);
                }
                streamWriter.Write(stringBuilder.ToString());
            }
        }
        //public string WriteCsv(List<ProductData> products)
        //{
        //    var stringBuilder = new StringBuilder();

        //    foreach (var productData in products)
        //    {
        //        string[] columns = GetColumns(productData);
        //        string line = string.Join(";", columns);

        //        stringBuilder.AppendLine(line);
        //    }
        //    return stringBuilder.ToString();
        //}
        //private string[] GetColumns(ProductData productData)
        //{
        //    string[] column = new string[13];

        //    column[0] = productData.Id;
        //    column[1] = productData.Name;
        //    column[2] = productData.DisplayName;
        //    column[3] = productData.AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss");

        //    Dictionary<string, object> properties = productData.Properties.ToDictionary(x => x.Name, x => x.Value);
            
        //    if (properties.ContainsKey("AvailableUntil"))
        //    {
        //        column[4] = properties["AvailableUntil"].ToString();
        //    }

        //    if (properties.ContainsKey("UnitPrice"))
        //    {
        //        column[5] = properties["UnitPrice"].ToString();
        //    }

        //    if (properties.ContainsKey("AvailableInMarkets"))
        //    {
        //        column[6] = properties["AvailableInMarkets"].ToString();
        //    }

        //    if (properties.ContainsKey("Sizes"))
        //    {
        //        column[7] = properties["Sizes"].ToString();
        //    }

        //    if (properties.ContainsKey("Description"))
        //    {
        //        column[8] = properties["Description"] as string;
        //    }

        //    if (properties.ContainsKey("DelieveryNote"))
        //    {
        //        column[9] = properties["DelieveryNote"].ToString();
        //    }

        //    if (properties.ContainsKey("DelieveryFromDays"))
        //    {
        //        column[10] = properties["DelieveryFromDays"].ToString();
        //    }

        //    if (properties.ContainsKey("DelieveryToDays"))
        //    {
        //        column[11] = properties["DelieveryToDays"].ToString();
        //    }

        //    if (properties.ContainsKey("ProductSoldOut"))
        //    {
        //        column[12] = properties["ProductSoldOut"].ToString();
        //    }

        //    return column;
        //}
    }
}
