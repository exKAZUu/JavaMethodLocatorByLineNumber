using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code2Xml.Core.Generators.ANTLRv4.Java;
using Code2Xml.Core.Location;
using Code2Xml.Core.SyntaxTree;
using Paraiba.Text;

namespace JavaMethodLocatorByLineNumber {
    public static class JavaMethodLocator {
        private static CstNode Locate(CstNode tree, string code, int lineNumber) {
            if (lineNumber <= 0) {
                return null;
            }

            var structuredCode = new StructuredCode(code);
            var line = structuredCode.GetLine(lineNumber);
            var startPos = 0;
            var endPos = line.Length;
            if (endPos == 0) {
                return null;
            }

            while (char.IsWhiteSpace(line[startPos])) {
                startPos++;
                if (startPos == endPos) {
                    return null;
                }
            }
            while (char.IsWhiteSpace(line[endPos - 1])) {
                endPos--;
                if (startPos == endPos) {
                    return null;
                }
            }
            var codeRange = new CodeRange(new CodeLocation(lineNumber, startPos),
                new CodeLocation(lineNumber, endPos));
            var node = codeRange.FindOutermostNode(tree);
            while (node != null && node.Name != "classBodyDeclaration") {
                node = node.Parent;
            }
            return node;
        }

        private static bool HasMethodName(CstNode node) {
            var firstChild = node.FirstChild;
            if (firstChild.Name == "classMemberDeclaration") {
                if (firstChild.FirstChild.Name == "methodDeclaration") {
                    return true;
                }
            } else if (firstChild.Name == "constructorDeclaration") {
                return true;
            }
            return false;
        }

        private static string GetFullMethodName(CstNode tree, CstNode node) {
            if (!HasMethodName(node)) {
                return null;
            }

            var names = new List<string>();
            if (tree.FirstChild.Name == "packageDeclaration") {
                names.AddRange(tree.FirstChild.Children("Identifier").Select(n => n.TokenText));
            }
            names.AddRange(
                node.Ancestors()
                        .Where(n => n.Name == "classDeclaration" || n.Name == "interfaceDeclaration")
                        .Select(n => n.Descendants("Identifier").First().TokenText));
            names.Add(node
                    .Descendants()
                    .Where(n => n.Name == "methodDeclarator" || n.Name == "constructorDeclarator")
                    .Descendants("Identifier")
                    .First().TokenText);
            return string.Join(".", names);
        }

        public static string GetFullMethodName(FileInfo fileInfo, int lineNumber) {
            var code = GuessEncoding.ReadAllText(fileInfo.FullName);
            var cstGen = new JavaCstGenerator();
            var tree = cstGen.GenerateTreeFromCodeText(code);
            var node = Locate(tree, code, lineNumber);
            if (node == null) {
                return null;
            }
            return GetFullMethodName(tree, node);
        }

        public static string GetFullMethodName(string code, int lineNumber) {
            var cstGen = new JavaCstGenerator();
            var tree = cstGen.GenerateTreeFromCodeText(code);
            var node = Locate(tree, code, lineNumber);
            if (node == null) {
                return null;
            }
            return GetFullMethodName(tree, node);
        }
    }
}