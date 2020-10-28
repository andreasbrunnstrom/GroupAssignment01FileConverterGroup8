using FileConverter.Models;
using System.IO;

namespace FileConverter.Data
{
    public interface IFileWriter : ITypeRegistration
    {
        void Write(Stream stream, object data);

    }
}
