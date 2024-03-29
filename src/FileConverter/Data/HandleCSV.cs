﻿using System;
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
            List<ProductData> products = new List<ProductData>();

            foreach (var row in rows)
            {
                try
                {
                    ProductData product = CreateProduct(row);
                    products.Add(product);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return products;
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

        private ProductData CreateProduct(string row)
        {
            string[] columns = row.Split(";");
            ProductData productData = new ProductData
            {
                Id = columns[0].ToString(),
                Name = columns[1].ToString()                
            };

            if (!string.IsNullOrEmpty(columns[2]))
            {
                productData.DisplayName = columns[2];
            }
            if (!string.IsNullOrEmpty(columns[3]))
            {
                productData.AvailableFrom = DateTime.Parse(columns[3], CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(columns[4]))
            {
                productData.AvailableUntil = DateTime.Parse(columns[4], CultureInfo.InvariantCulture);
            }

            PropertyData property;           
            if (!string.IsNullOrEmpty(columns[5]))
            {
                productData.UnitPrice.Amount = decimal.Parse(columns[5]);
                productData.UnitPrice.Currency = "SEK";
            }
            if (!string.IsNullOrEmpty(columns[6]))
            {
                var hold = columns[6].Split(",");
                foreach (var el in hold)
                {
                    productData.AvailableInMarkets.Add(el);
                }
            }
            if (!string.IsNullOrEmpty(columns[7]))
            {
                var hold = columns[7].Split(",");
                foreach (var el in hold)
                {
                    productData.Sizes.Add(el);
                }             
            }                        
            if (!string.IsNullOrEmpty(columns[8]))
            {
                property = CreateStringProperty("Description", columns[8]);
                string newLine = AddNewLine(property.Value.ToString());
                property.Value = newLine;
                productData.Properties.Add(property);
            }
            if (!string.IsNullOrEmpty(columns[9]))
            {
                property = CreateStringProperty("DelieveryNote", columns[9]);
                string newLine = AddNewLine(property.Value.ToString());
                property.Value = newLine;
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
                property = CreateBoolProperty("ProductSoldOut", columns[12]);
                productData.Properties.Add(property);
            }
            
            return productData;
        }

        public void Write(Stream stream, object data)
        {
            List<ProductData> products = new List<ProductData>();
            products = data as List<ProductData>;
            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
                var stringBuilder = new StringBuilder();
                foreach (var product in products)
                {
                    string[] columns = GetColumns(product);
                    string line = string.Join(";", columns);
                    stringBuilder.AppendLine(line);
                }
                streamWriter.Write(stringBuilder.ToString());
            }
        }

        private string[] GetColumns(ProductData product)
        {
            string[] result = new string[13];
            result[0] = product.Id.ToString();
            result[1] = product.Name.ToString();
            if (!string.IsNullOrEmpty(product.DisplayName)) { result[2] = product.DisplayName; }
            result[3] = product.AvailableFrom.ToString();
            if (product.AvailableUntil.HasValue) { result[4] = product.AvailableUntil.ToString(); }
            result[5] = Math.Round(product.UnitPrice.Amount, 0).ToString();
            //result[6] = product.UnitPrice.Currency;

            string markets = "";
            int count = 0;
            foreach (var market in product.AvailableInMarkets)
            {
                if (count < 1)
                { markets = market; }
                else
                { markets += "," + market; }
                count++;
            }

            result[6] = markets;

            string sizes = "";
            count = 0;
            foreach (var size in product.Sizes)
            {
                if (count < 1)
                { sizes = size; }
                else
                { sizes += "," + size; }
                count++;
            }

            result[7] = sizes;

            Dictionary<string, object> properties = product.Properties.ToDictionary(x => x.Name, x => x.Value);

            if (properties.ContainsKey("Description"))
            {
                string cleaned = RemoveNewLine(properties["Description"].ToString());
                result[8] = cleaned;

            }
            if (properties.ContainsKey("DelieveryNote"))
            {
                string cleaned = RemoveNewLine(properties["DelieveryNote"].ToString());
                result[9] = cleaned;
            }
            if (properties.ContainsKey("DeliveryFromDays")) { result[10] = properties["DeliveryFromDays"].ToString(); }
            if (properties.ContainsKey("DeliveryToDays")) { result[11] = properties["DeliveryToDays"].ToString(); }
            if (properties.ContainsKey("ProductSoldOut")) { result[12] = properties["ProductSoldOut"].ToString(); }

            return result;
        }

        private string RemoveNewLine(string input)
        {
            string cleaned = input.Replace("\r\n", "|").Replace("\n", "|");
            return cleaned;
        }

        private string AddNewLine(string input)
        {
            string addedNewLine = input.Replace("|", "\r\n").Replace("|", "\n");
            return addedNewLine;
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
    }
}
