using FileConverter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FileConverter.Data
{
    class HandleBIN : IFileHandler
    {
        public string Extension { get; } = ".bin";

        public object Read(Stream fileStream)
        {
            try
            {
                BinaryFormatter binary = new BinaryFormatter();
                var parsedBin = binary.Deserialize(fileStream) as List<ProductData>;
                return parsedBin;
            }
            catch
            {
                throw;
            }
        }

        public void Write(Stream stream, object data)
        {
            BinaryFormatter binary = new BinaryFormatter();
            binary.Serialize(stream, data);
        }
    }
}
