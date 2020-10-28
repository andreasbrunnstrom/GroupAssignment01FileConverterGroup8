using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;


namespace FileConverter.Tests
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void CanRunXmlToJson()
        {
            CanRun(".xml", "temp.json");
        }

        [TestMethod]
        public void CanRunJsonToBin()
        {
            CanRun(".json", "temp.bin");
        }

        [TestMethod]
        public void CanRunCsvToXml()
        {
            CanRun(".csv", "temp.xml");
        }

        private void CanRun(string inputExtension, string outputFile)
        {
            var options = new Options
            {
                Input = GetPath($"Template{inputExtension}"),
                Output = GetPath(outputFile)
            };
           

            var file = new FileInfo(outputFile);

            Assert.IsTrue(file.Exists);
            Assert.IsTrue(file.Length > 0);

            if (file.Exists)
                File.Delete(outputFile);
        }

        private string GetPath(string fileName)
        {
            return Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, fileName);
        }
    }
}
