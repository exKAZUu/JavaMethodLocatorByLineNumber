using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code2Xml.Core.Generators.ANTLRv4.Java;
using Code2Xml.Core.Location;
using Code2Xml.Core.SyntaxTree;
using Paraiba.Core;
using Paraiba.Text;

namespace JavaMethodLocatorByLineNumber {
    public static class JavaMethodLocator {
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

        private static CstNode LocateConstructorOrMethod(CstNode tree, string code, int lineNumber) {
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
            if (node == null || !HasMethodName(node)) {
                return null;
            }

            return node;
        }

        private static string GetFullMethodNameWithParameterTypes(CstNode tree, CstNode node) {
            var names = new List<string>();
            if (tree.FirstChild.Name == "packageDeclaration") {
                names.AddRange(tree.FirstChild.Children("Identifier").Select(n => n.TokenText));
            }
            names.AddRange(
                node.Ancestors()
                        .Where(n => n.Name == "classDeclaration" || n.Name == "interfaceDeclaration")
                        .Select(n => n.Descendants("Identifier").First().TokenText));
            var declarator = node
                    .Descendants()
                    .First(n => n.Name == "methodDeclarator" || n.Name == "constructorDeclarator");
            names.Add(declarator
                    .Descendants("Identifier")
                    .First().TokenText);
            return Enumerable.Repeat(string.Join(".", names), 1)
                    .Concat(declarator.Descendants("formalParameter")
                            .Select(n => n.Children("unannType").First().TokenText))
                    .JoinString(",");
        }

        public static string GetFullMethodNameWithParameterTypes(FileInfo fileInfo, int lineNumber) {
            var code = GuessEncoding.ReadAllText(fileInfo.FullName);
            var cstGen = new JavaCstGenerator();
            var tree = cstGen.GenerateTreeFromCodeText(code);
            var node = LocateConstructorOrMethod(tree, code, lineNumber);
            if (node == null) {
                return null;
            }
            return GetFullMethodNameWithParameterTypes(tree, node);
        }

        public static string GetFullMethodNameWithParameterTypes(string code, int lineNumber) {
            var cstGen = new JavaCstGenerator();
            var tree = cstGen.GenerateTreeFromCodeText(code);
            var node = LocateConstructorOrMethod(tree, code, lineNumber);
            if (node == null) {
                return null;
            }
            return GetFullMethodNameWithParameterTypes(tree, node);
        }

        private static IEnumerable<CstNode> FindConstructorsAndMethods(CstNode tree) {
            return tree.Descendants().Where(node =>
                        node.Name == "methodDeclaration" || node.Name == "constructorDeclaration");
        }

        public static IEnumerable<Tuple<CodeRange, string>>
                GetCodeRangeAndFullNameWihtParameterTypes(string code) {
            var cstGen = new JavaCstGenerator();
            var tree = cstGen.GenerateTreeFromCodeText(code);
            return FindConstructorsAndMethods(tree).Select(node =>
                Tuple.Create(CodeRange.Locate(node),
                    GetFullMethodNameWithParameterTypes(tree, node)));
        }

        public static IEnumerable<Tuple<CodeRange, string>>
                GetCodeRangeAndFullNameWihtParameterTypes(FileInfo fileInfo) {
            var code = GuessEncoding.ReadAllText(fileInfo.FullName);
            return GetCodeRangeAndFullNameWihtParameterTypes(code);
        }
    }
}