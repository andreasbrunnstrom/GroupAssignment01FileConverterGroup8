using System;
using System.Collections.Generic;
using System.Text;
using FileConverter.Models;

namespace FileConverter.Data
{
    interface IFileHandler : IFileReader, IFileWriter
    {
    }
}
