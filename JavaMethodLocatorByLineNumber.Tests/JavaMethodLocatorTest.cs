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
        public void Test() {
            var node = JavaMethodLocator.Locate(@"//test
import javax.swing.*;
 
public class Hello extends JFrame {
    Hello() /*test*/ {
        setDefaultCloseOperation(WindowConstants.DISPOSE_ON_CLOSE);
        pack(); // pack();
    }
 
    public static void main(String[] args) {
        new Hello().setVisible(true);
    }
}", 10);
            var methodName = JavaMethodLocator.GetMethodName(node);
            Assert.That(methodName, Is.EqualTo("main"));
        }
    }
}
