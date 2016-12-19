using System;
using System.IO;

namespace JavaMethodLocatorByLineNumber {
    public class Program {
        static void ExitShowingUsage() {
            Console.WriteLine("Usage: JavaMethodLocatorByLineNumber <file_path> <line_number>");
            Environment.Exit(1);
        }

        static void Main(string[] args) {
            if (args.Length < 2) {
                ExitShowingUsage();
            }
            var fileInfo = new FileInfo(args[0]);
            if (!fileInfo.Exists) {
                ExitShowingUsage();
            }
            var lineNumber = 0;
            if (!int.TryParse(args[1], out lineNumber)) {
                ExitShowingUsage();
            }

            var node = JavaMethodLocator.Locate(fileInfo, lineNumber);
            Console.WriteLine(node.Name);
        }
    }
}