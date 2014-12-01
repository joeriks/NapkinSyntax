using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

namespace Napkin.NewSyntaxTests
{
    public class SomeNode
    {
        public string Name { get; set; }
        public string Attr { get; set; }
        public string Id { get; set; }
        public IEnumerable<SomeNode> Children { get; set; }
        public SomeNode()
        {
            Children = new List<SomeNode>();
        }
    }
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void FullSyntax_Indentation_Linq_Recursive()
        {
            var x = new Node(@"
Node
    Name=Foo
    Id=2
Node
    Name=Bar
    Id=1
    Node
        Name=Baz
        Id=3", new UnfoldSettings
            {
                EachHeaderHasTypeName = true,
                Style = Style.Indent
            });

            Func<Node, SomeNode> selector = null;
            selector = new Func<Node, SomeNode>(t => new SomeNode
            {
                Name = t.GetPropertyValue<string>("Name"),
                Id = t.GetPropertyValue<string>("Id"),
                Children = t.Children.Where(c => c.HeaderName == "Node").Select(selector)
            });

            var nodes = x.Children.Where(c => c.HeaderName == "Node").Select(selector);

            Assert.AreEqual(2, nodes.Count());
            Assert.AreEqual("3", nodes.ElementAt(1).Children.ElementAt(0).Id);

        }

        [TestMethod]
        public void ShortSyntax_Indentation_Linq_Recursive()
        {
            var x = new Node(@"
Node
    Name=Foo
    Id=2
Node
    Name=Bar
    Id=1
    Node
        Name=Baz
        Id=3");

            
            Func<Node, SomeNode> nodeSelector = null;
            nodeSelector = new Func<Node, SomeNode>(t => new SomeNode
            {
                Name = t.Properties["Name"],
                Id = t.Properties["Id"],
                Children = t.Children.Where(c => c.HeaderName == "Node").Select(nodeSelector)
            });

            var nodes = x.Children.Where(c => c.HeaderName == "Node").Select(nodeSelector);

            Assert.AreEqual(2, nodes.Count());
            Assert.AreEqual("3", nodes.ElementAt(1).Children.ElementAt(0).Id);

        }


        [TestMethod]
        public void WithAttributes_Indentation_Linq_Recursive()
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


            Func<Node, SomeNode> nodeSelector = null;
            nodeSelector = new Func<Node, SomeNode>(t => new SomeNode
            {
                Name = t.Properties["Name"],
                Id = t.Properties["Id"],
                Children = t.Children.Where(c => c.HeaderName == "Node").Select(nodeSelector)
            });

            var nodes = x.Children.Where(c => c.HeaderName == "Node").Select(nodeSelector);

            Assert.AreEqual(2, nodes.Count());
            Assert.AreEqual("3", nodes.ElementAt(1).Children.ElementAt(0).Id);

        }




    }
}
