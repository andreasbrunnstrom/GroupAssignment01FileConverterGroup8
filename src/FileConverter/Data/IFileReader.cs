using FileConverter.Models;
using System.IO;

namespace FileConverter.Data
{
    public interface IFileReader : ITypeRegistration
    {
        object Read(Stream fileStream);

    }
}
