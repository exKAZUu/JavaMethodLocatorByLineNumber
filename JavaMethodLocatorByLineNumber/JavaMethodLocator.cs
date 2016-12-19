using System.IO;
using System.Linq;
using Code2Xml.Core.Generators.ANTLRv4.Java;
using Code2Xml.Core.Location;
using Code2Xml.Core.SyntaxTree;
using Paraiba.Text;

namespace JavaMethodLocatorByLineNumber {
    public static class JavaMethodLocator {
        public static CstNode Locate(FileInfo fileInfo, int lineNumber) {
            var code = GuessEncoding.ReadAllText(fileInfo.FullName);
            return Locate(code, lineNumber);
        }

        public static CstNode Locate(string code, int lineNumber) {
            var cstGen = new JavaCstGenerator();
            var tree = cstGen.GenerateTreeFromCodeText(code);
            var structuredCode = new StructuredCode(code);
            var line = structuredCode.GetLine(lineNumber);
            var startPos = 0;
            var endPos = line.Length;
            while (char.IsWhiteSpace(line[startPos])) {
                startPos++;
            }
            while (char.IsWhiteSpace(line[endPos - 1])) {
                endPos--;
            }
            var codeRange = new CodeRange(new CodeLocation(lineNumber, startPos),
                new CodeLocation(lineNumber, endPos));
            return codeRange.FindOutermostNode(tree);
        }

        public static string GetMethodName(CstNode node) {
            return node.Descendants("Identifier").First().TokenText;
        }
    }
}