using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace JavaMethodLocatorByLineNumber.Tests
{
    [TestFixture]
    public class JavaMethodLocatorTest
    {
        [Test]
        [TestCase(0, ExpectedResult = null)]
        [TestCase(1, ExpectedResult = null)]
        [TestCase(2, ExpectedResult = null)]
        [TestCase(3, ExpectedResult = null)]
        [TestCase(4, ExpectedResult = null)]
        [TestCase(5, ExpectedResult = null)]
        [TestCase(6, ExpectedResult = "com.test.hello.Hello.Hello")]
        [TestCase(7, ExpectedResult = "com.test.hello.Hello.Hello")]
        [TestCase(8, ExpectedResult = "com.test.hello.Hello.Hello")]
        [TestCase(9, ExpectedResult = "com.test.hello.Hello.Hello")]
        [TestCase(10, ExpectedResult = null)]
        [TestCase(11, ExpectedResult = "com.test.hello.Hello.main")]
        [TestCase(12, ExpectedResult = "com.test.hello.Hello.main")]
        [TestCase(13, ExpectedResult = "com.test.hello.Hello.main")]
        [TestCase(14, ExpectedResult = null)]
        public string Test(int lineNumber) {
            return JavaMethodLocator.GetFullMethodName(@"//test
package com.test.hello;
import javax.swing.*;
 
public class Hello extends JFrame {
    Hello() /*test*/ {
        setDefaultCloseOperation(WindowConstants.DISPOSE_ON_CLOSE);
        pack(); // pack();
    }
 
    public static void main(String[] args) {
        new Hello().setVisible(true);
    }
}", lineNumber);
        }
    }
}
