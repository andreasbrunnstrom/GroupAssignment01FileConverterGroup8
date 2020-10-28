using FileConverter.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace FileConverter.Data
{
    class HandleJSON : IFileHandler
    {
        public string Extension { get; } = ".json";

        public object Read(Stream fileStream)
        {
            try
            {
                using (StreamReader reader = new StreamReader(fileStream))
                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                {
                    JsonSerializer ser = new JsonSerializer();
                    return ser.Deserialize<List<ProductData>>(jsonReader);
                }
            }
            catch
            {
                throw;
            }
        }

        public void Write(Stream stream, object data)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer ser = new JsonSerializer();                
                ser.Serialize(jsonWriter, data);
                jsonWriter.Flush();
            }
        }
    }
}
