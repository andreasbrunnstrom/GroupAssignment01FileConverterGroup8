using CommandLine;
using FileConverter.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileConverter
{
        public class Options
        {
        [Option('i', "input", Required = true, HelpText = "Path to input file.")]
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "File path to output to.")]
        public string Output { get; set; }


        public class FileConverterProgram
        {
            static void Main(string[] args)
            {
                Parser.Default.ParseArguments<Options>(args)
                              .WithParsed(Run)
                              .WithNotParsed(Abort);
            }

            public static void Run(Options options)
            {
                IFileHandler inputHandler = null;
                IFileHandler outputHandler = null;

                string inputExtension = Path.GetExtension(options.Input);
                string outputExtension = Path.GetExtension(options.Output);

                List<string> supportedExtensions = new List<string>();

                List<IFileHandler> handlers = new List<IFileHandler>();
                {
                    new HandleCSV();
                    new HandleBIN();
                    new HandleJSON();
                    new HandleXML();
                }
                foreach (var handler in handlers)
                {
                    if (inputExtension == handler.Extension)
                    {
                        inputHandler = handler;
                    }
                    if(outputExtension == handler.Extension)
                    {
                        outputHandler = handler;
                    }
                    supportedExtensions.Add(handler.Extension);
                }
                if (!supportedExtensions.Contains(inputExtension) || !supportedExtensions.Contains(outputExtension))
                    throw new ArgumentException("Extension not supported.");

                if ((inputHandler != null) && (outputHandler != null))
                {
                    FileStream input = new FileStream(options.Input, FileMode.Open);
                    FileStream output = new FileStream(options.Output, FileMode.Create);

                    object deserializedData = inputHandler.Read(input);
                    outputHandler.Write(output, deserializedData);

                    output.Close();
                    input.Close();
                }

            }
            
        }
        static void Abort(IEnumerable<Error> errors)
        {
            foreach (var error in errors)
            {
                Console.WriteLine(error.Tag);
            }
        }
    }
}        
