using System;
using System.Diagnostics;
using System.IO;

namespace TestingApp
{
    static class Program
    {
        public static void About()
        {
            Console.WriteLine("TestingApp Version 1.1 Copyright (c) 2016 Konstantinov Ostap");
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
            Console.WriteLine("Usage: TestingApp <command> [application] [argument]\n"
                + "Commands: -h Help\n"
                + "Examples: TestingApp -h\n"
                + "TestingApp.exe TextToASCII.exe 10");
        }

        private static bool FileCompare(string file1, string file2)
        {
            if (file1 == file2)
                return true;

            int file1Byte;
            int file2Byte;

            using (FileStream fs1 = File.OpenRead(file1),
                fs2 = File.OpenRead(file2))
            {
                if (fs1.Length != fs2.Length)
                    return false;

                do
                {
                    file1Byte = fs1.ReadByte();
                    file2Byte = fs2.ReadByte();
                }
                while ((file1Byte == file2Byte) && (file1Byte != -1));
            }

            return (file1Byte == file2Byte);
        }

        public static void FullLog(string appName, string argStr)
        {
            if (!File.Exists(appName))
            {
                InvalidCommand();
                return;
            }

            if (!Directory.Exists("Output") && Directory.Exists("Input"))
                Directory.CreateDirectory("Output");

            uint good = 0, bad = 0, miss = 0;
            DateTime start = DateTime.Now;

            foreach (string iFile in Directory.EnumerateFiles("Input", "*.txt"))
            {
                string iFileN = Path.GetFileName(iFile);

                string oFile = "Output\\" + iFileN;
                string eFile = "Etalon\\" + iFileN;

                using (Process p = new Process())
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.FileName = appName;
                    p.StartInfo.Arguments = argStr + " " + iFile + " " + oFile;
                    p.Start();
                    p.WaitForExit();
                }

                string test = iFileN + ": ";

                if (File.Exists(eFile))
                    if (FileCompare(oFile, eFile))
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
                            InvalidArgument();
                            break;
                    }
                    break;

                case 2:
                    FullLog(args[0], args[1]);
                    break;

                default:
                    InvalidArgument();
                    break;
            }
        }
    }
}
