﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace TestingApp
{
    class Program
    {
        public static void About()
        {
            Console.WriteLine("Tester Version 1.1 Copyright (c) 2016 Konstantinov Ostap");
        }

        public static void InvalidArgument()
        {
            Console.Error.WriteLine("Error: Invalid Argument");
        }

        public static void InvalidCommand()
        {
            Console.Error.WriteLine("Error: Invalid Command");
        }

        public static void Help()
        {
            Console.WriteLine("Usage: Tester [command]");
            Console.WriteLine("Commands: -h Help");
            Console.WriteLine("Examples: TestingApp -h");
            Console.WriteLine("          TestingApp TextToASCII.exe");
        }

        private static bool FileCompare(string file1, string file2)
        {
            if (file1 == file2)
                return true;

            int file1byte;
            int file2byte;

            using (FileStream fs1 = File.OpenRead(file1),
                fs2 = File.OpenRead(file2))
            {
                if (fs1.Length != fs2.Length)
                    return false;

                do
                {
                    file1byte = fs1.ReadByte();
                    file2byte = fs2.ReadByte();
                }
                while ((file1byte == file2byte) && (file1byte != -1));
            }

            return (file1byte == file2byte);
        }

        public static void FullLog(string testAppName)
        {
            if (!File.Exists(testAppName))
            {
                InvalidCommand();
                return;
            }

            const string inputFolder = "Input";
            const string outputFolder = "Output";
            const string etalonFolder = "Etalon";

            if (!Directory.Exists(outputFolder) && Directory.Exists(inputFolder))
                Directory.CreateDirectory(outputFolder);

            uint good = 0, bad = 0, miss = 0;
            DateTime start = DateTime.Now;

            foreach (string inputFile in Directory.EnumerateFiles(inputFolder, "*.txt"))
            {
                string inputFileName = Path.GetFileName(inputFile);

                string outputFile = outputFolder + "\\" + inputFileName;
                string etalonFile = etalonFolder + "\\" + inputFileName;

                using (Process proc = new Process())
                {
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = testAppName;
                    proc.StartInfo.Arguments = inputFile + " " + outputFile;
                    proc.Start();
                    proc.WaitForExit();
                }

                string test = inputFileName + ": ";

                if (File.Exists(etalonFile))
                    if (FileCompare(outputFile, etalonFile))
                    {
                        test += "OK";
                        good++;
                    }
                    else
                    {
                        test += "ERROR";
                        bad++;
                    }
                else
                {
                    test += "NOT TESTED";
                    miss++;
                }

                Console.WriteLine(test);
            }

            Console.WriteLine();
            Console.WriteLine("Successful: " + good);
            Console.WriteLine("Failed: " + bad);
            Console.WriteLine("Skipped: " + miss);
            Console.WriteLine("Spent time: " + (DateTime.Now - start).TotalMilliseconds + "ms");
        }

        static void Main(string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    About();
                    break;

                case 1:
                    switch (args[0])
                    {
                        case "-h":
                            Help();
                            break;
                        default:
                            FullLog(args[0]);
                            break;
                    }
                    break;

                default:
                    InvalidArgument();
                    break;
            }
        }
    }
}
