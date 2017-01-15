using System.Linq;
using NUnit.Framework;

namespace JavaMethodLocatorByLineNumber.Tests {
    [TestFixture]
    public class JavaMethodLocatorTest {
        [Test]
        [TestCase(0, ExpectedResult = null)]
        [TestCase(1, ExpectedResult = null)]
        [TestCase(2, ExpectedResult = null)]
        [TestCase(3, ExpectedResult = null)]
        [TestCase(4, ExpectedResult = null)]
        [TestCase(5, ExpectedResult = null)]
        [TestCase(6, ExpectedResult = "com.test.hello.Hello.Hello,int,int")]
        [TestCase(7, ExpectedResult = "com.test.hello.Hello.Hello,int,int")]
        [TestCase(8, ExpectedResult = "com.test.hello.Hello.Hello,int,int")]
        [TestCase(9, ExpectedResult = "com.test.hello.Hello.Hello,int,int")]
        [TestCase(10, ExpectedResult = null)]
        [TestCase(11, ExpectedResult = "com.test.hello.Hello.main,String[]")]
        [TestCase(12, ExpectedResult = "com.test.hello.Hello.main,String[]")]
        [TestCase(13, ExpectedResult = "com.test.hello.Hello.main,String[]")]
        [TestCase(14, ExpectedResult = "com.test.hello.Hello.main,String[]")]
        [TestCase(15, ExpectedResult = null)]
        public string GetFullMethodNameByLineNumber(int lineNumber) {
            return JavaMethodLocator.GetFullMethodNameWithParameterTypes(@"//test
package com.test.hello;
import javax.swing.*;
 
public class Hello extends JFrame {
    Hello(int a, int b) /*test*/ {
        setDefaultCloseOperation(WindowConstants.DISPOSE_ON_CLOSE);
        pack(); // pack();
    }
 
    @Override
    public static void main(String[] args) {
        new Hello().setVisible(true);
    }
}", lineNumber);
        }

        [Test]
        public void ListUpCodeRangeAndFullMethodName() {
            var rangeAndNames = JavaMethodLocator.GetCodeRangeAndFullNameWihtParameterTypes(@"//test
package com.test.hello;
 
public class Hello {
    Hello() { }

    @Override public static void main(String[] args) {
        new Hello().setVisible(true);
    }

    class World {
        void someMethod(int a, String ... ss) { }
    }
}").ToList();
            Assert.That(rangeAndNames.Count, Is.EqualTo(3));
            Assert.That(rangeAndNames[0].Item1.StartLine, Is.EqualTo(5));
            Assert.That(rangeAndNames[0].Item1.EndLine, Is.EqualTo(5));
            Assert.That(rangeAndNames[0].Item2, Is.EqualTo("com.test.hello.Hello.Hello"));
            Assert.That(rangeAndNames[1].Item1.StartLine, Is.EqualTo(7));
            Assert.That(rangeAndNames[1].Item1.EndLine, Is.EqualTo(9));
            Assert.That(rangeAndNames[1].Item2, Is.EqualTo("com.test.hello.Hello.main,String[]"));
            Assert.That(rangeAndNames[2].Item1.StartLine, Is.EqualTo(12));
            Assert.That(rangeAndNames[2].Item1.EndLine, Is.EqualTo(12));
            Assert.That(rangeAndNames[2].Item2, Is.EqualTo("com.test.hello.Hello.World.someMethod,int,String[]"));
        }
    }
}