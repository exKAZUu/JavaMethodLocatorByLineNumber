using System;
using System.IO;

namespace JavaMethodLocatorByLineNumber {
    public class Program {
        static void ExitShowingUsage() {
            Console.WriteLine("Usage: JavaMethodLocatorByLineNumber <file_path> [<line_number>]");
            Environment.Exit(1);
        }

        static void Main(string[] args) {
            if (args.Length < 1) {
                ExitShowingUsage();
            }
            var fileInfo = new FileInfo(args[0]);
            if (!fileInfo.Exists) {
                ExitShowingUsage();
            }
            if (args.Length >= 2) {
                var lineNumber = 0;
                if (!int.TryParse(args[1], out lineNumber)) {
                    ExitShowingUsage();
                }

                var fullName = JavaMethodLocator.GetFullMethodNameWithParameterTypes(fileInfo, lineNumber);
                if (fullName == null) {
                    Environment.Exit(-1);
                }
                Console.WriteLine(fullName);
            } else {
                foreach (var rangeAndName in JavaMethodLocator.GetCodeRangeAndFullNameWihtParameterTypes(fileInfo)) {
                    Console.WriteLine("{0},{1},{2}",
                        rangeAndName.Item1.StartLine, rangeAndName.Item1.EndLine, rangeAndName.Item2);
                }
            }
        }
    }
}