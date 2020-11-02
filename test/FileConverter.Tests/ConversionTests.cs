using System;
using System.IO;
using FileConverter.Services;
using FileConverter.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeepEqual.Syntax;
using DeepEqual;

namespace FileConverter.Tests
{
    [TestClass]
    public class ConversionTests
    {
        [TestMethod]
        public void HasXmlReader()
        {
            HasReader(".xml");
        }

        [TestMethod]
        public void HasXmlWriter()
        {
            HasWriter(".xml");
        }

        [TestMethod]
        public void CanConvertXml()
        {
            CanConvert(".xml");
        }

        [TestMethod]
        public void CanConvertCsvToXml()
        {
            CanConvert(".xml", ".csv");
        }

        [TestMethod]
        public void CanConvertJsonToXml()
        {
            CanConvert(".xml", ".json");
        }

        [TestMethod]
        public void HasJsonReader()
        {
            HasReader(".json");
        }

        [TestMethod]
        public void HasJsonWriter()
        {
            HasWriter(".json");
        }

        [TestMethod]
        public void CanConvertJson()
        {
            CanConvert(".json");
        }

        [TestMethod]
        public void CanConvertXmlToJson()
        {
            CanConvert(".json", ".xml");
        }

        [TestMethod]
        public void CanConvertCsvToJson()
        {
            CanConvert(".json", ".csv");
        }

        [TestMethod]
        public void HasBinaryReader()
        {
            HasReader(".bin");
        }

        [TestMethod]
        public void HasBinaryWriter()
        {
            HasWriter(".bin");
        }

        [TestMethod]
        public void CanConvertXmlToBinary()
        {
            CanConvert(".bin", ".xml");
        }

        [TestMethod]
        public void HasCsvReader()
        {
            HasReader(".csv");
        }

        [TestMethod]
        public void HasCsvWriter()
        {
            HasWriter(".csv");
        }

        [TestMethod]
        public void CanConvertCsv()
        {
            CanConvert(".csv");
        }

        [TestMethod]
        public void CanConvertJsonToCsv()
        {
            CanConvert(".csv", ".json");
        }

        [TestMethod]
        public void CanConvertXmlToCsv()
        {
            CanConvert(".csv", ".xml");
        }

        private void CanConvert(string extension, string sourceExtension = null)
        {
            if (sourceExtension != null)
                HasReader(sourceExtension);

            HasReader(extension);
            HasWriter(extension);

            var path = GetTemplatePath(sourceExtension ?? extension);
            var sourceReader = GetReader(sourceExtension ?? extension);
            var reader = GetReader(extension);
            var writer = GetWriter(extension);

            object firstRead;
            object secondRead;

            using (var inputStream = File.OpenRead(path))
            {
                firstRead = sourceReader.Read(inputStream);

                Assert.IsNotNull(firstRead);

                using (var outputBuffer = new MemoryStream())
                {
                    writer.Write(outputBuffer, firstRead);

                    var bytes = outputBuffer.ToArray();
                    var bytesLargerThanZero = bytes.Length > 0;

                    Assert.IsTrue(bytesLargerThanZero);

                    var inputBuffer = new MemoryStream(bytes);

                    secondRead = reader.Read(inputBuffer);
                }
            }

            firstRead.ShouldDeepEqual(secondRead, GetComparisonRules());
        }

        private void HasWriter(string extension)
        {
            var writers = ConverterRegistrationScanner.SupportedWriters;
            Assert.IsTrue(writers.ContainsKey(extension));
        }

        private void HasReader(string extension)
        {
            var readers = ConverterRegistrationScanner.SupportedReaders;
            Assert.IsTrue(readers.ContainsKey(extension));
        }

        private string GetTemplatePath(string extension)
        {
            var executingFolder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            return Path.Combine(executingFolder, "Template" + extension);
        }

        private IFileWriter GetWriter(string path)
        {
            var extension = Path.GetExtension(path);
            return ConverterRegistrationScanner.SupportedWriters[extension];
        }

        private IFileReader GetReader(string path)
        {
            var extension = Path.GetExtension(path);
            return ConverterRegistrationScanner.SupportedReaders[extension];
        }

        private IComparison GetComparisonRules()
        {
            ComparisonBuilder comparisonBuilder = new ComparisonBuilder();

            comparisonBuilder.CustomComparisons.Add(new LenientStringComparison());
            comparisonBuilder.CustomComparisons.Add(new LenientDateTimeComparison());

            return comparisonBuilder.Create();
        }

        class LenientStringComparison : IComparison
        {
            public bool CanCompare(Type type1, Type type2)
            {
                if (type1 != typeof(string)) return false;
                if (type2 != typeof(string)) return false;

                return true;
            }

            public (ComparisonResult result, IComparisonContext context) Compare(IComparisonContext context, object value1, object value2)
            {
                var string1 = (string)value1 ?? string.Empty;
                var string2 = (string)value2 ?? string.Empty;

                var equals = StripCarriageReturn(string1) == StripCarriageReturn(string2);
                var result = equals ? ComparisonResult.Pass : ComparisonResult.Fail;

                if (!equals)
                {
                    context.AddDifference(string1, string2);
                }

                return (result, context);
            }

            private string StripCarriageReturn(string input)
            {
                return input.Replace("\r", string.Empty);
            }
        }

        class LenientDateTimeComparison : IComparison
        {
            public bool CanCompare(Type type1, Type type2)
            {
                if (type1 != typeof(DateTime)) return false;
                if (type2 != typeof(DateTime)) return false;

                return true;
            }

            public (ComparisonResult result, IComparisonContext context) Compare(IComparisonContext context, object value1, object value2)
            {
                var date1 = (DateTime)value1;
                var date2 = (DateTime)value2;

                var equals = GetCoarseDate(date1) == GetCoarseDate(date2);
                var result = equals ? ComparisonResult.Pass : ComparisonResult.Fail;

                if (!equals)
                {
                    context.AddDifference(date1, date2);
                }

                return (result, context);
            }

            private DateTime GetCoarseDate(DateTime date)
            {
                return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
            }
        }
    }
}
