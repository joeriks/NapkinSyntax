using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

namespace Napkin.NewSyntaxTests
{
    [TestClass]
    public class FirstTests
    {


        [TestMethod]
        public void WithAttributes_Indentation()
        {
            var x = new Node(@"
Node Attr=x
    Name=Foo
    Id=2
Node Attr=y
    Name=Bar
    Id=1
    Node
        Name=Baz
        Id=3");


            Assert.AreEqual(2, x.Children.Count());

            var child = x.Children.First();

            Assert.AreEqual(@"    Name=Foo
    Id=2
", x.Children.First().RawDocument);

            Assert.IsFalse(child.IsUnfolded);

            var props = child.Properties;

            Assert.IsTrue(child.IsUnfolded);

            Assert.AreEqual(3, props.Count());

            Assert.AreEqual("x", child.GetPropertyValue<string>("Attr"));
            Assert.AreEqual("Foo", child.GetPropertyValue<string>("Name"));
            Assert.AreEqual("2", child.GetPropertyValue<string>("Id"));

        }




    }
}
