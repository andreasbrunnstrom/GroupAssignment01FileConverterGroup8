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
            CanRun(".json", "temp.bin", shouldBeBinary: true);
        }

        [TestMethod]
        public void CanRunCsvToXml()
        {
            CanRun(".csv", "temp.xml");
        }

        [TestMethod]
        public void CanRunJsonToCsv()
        {
            CanRun(".json", "temp.csv");
        }

        private void CanRun(string inputExtension, string outputFile, bool shouldBeBinary = false)
        {
            var options = new Options
            {
                Input = GetPath($"Template{inputExtension}"),
                Output = GetPath(outputFile)
            };

            Options.FileConverterProgram.Run(options);

            var file = new FileInfo(outputFile);

            Assert.IsTrue(file.Exists);
            Assert.IsTrue(file.Length > 0);

            var isBinary = IsBinary(file);

            Assert.AreEqual(isBinary, shouldBeBinary);

            if (file.Exists)
                File.Delete(outputFile);
        }

        private string GetPath(string fileName)
        {
            return Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, fileName);
        }

        public static bool IsBinary(FileInfo file)
        {
            if (file.Exists == false) return false;
            if (file.Length == 0) return false;

            using (var stream = new StreamReader(file.FullName))
            {
                int ch;
                while ((ch = stream.Read()) != -1)
                {
                    if (IsControlChar(ch))
                        return true;
                }
            }
            return false;
        }

        public static bool IsControlChar(int ch)
        {
            return ch > Chars.Null && ch < Chars.Backspace
                || ch > Chars.CarriageReturn && ch < Chars.Substitute;
        }

        public static class Chars
        {
            public const char Null = (char)0;
            public const char Backspace = (char)8;
            public const char CarriageReturn = (char)13;
            public const char Substitute = (char)26;
        }
    }
}
