using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

namespace Napkin.NewSyntaxTests
{
    [TestClass]
    public class BasicTests_HandlingSyntaxProblems
    {
        [TestMethod]
        public void FullSyntax_Indentation_Linq_Recursive()
        {
            var x = new Node(@"
/* totally unrelated */
Node
    Name=Foo
    Id=2
Node
    Name=Bar
    Id=1
    What is this even I cannot understand
    Node
        Name=Baz
        Id=3
        Also this should not be read
", new UnfoldSettings
            {
                EachHeaderHasTypeName = true,
                Style = Style.Indent
            });

            Func<Node, SomeNode> selector = null;
            selector = new Func<Node, SomeNode>(t => new SomeNode
            {
                Name = t.GetPropertyValue<string>("Name"),
                Id = t.GetPropertyValue<string>("Id"),
                Attr = t.GetPropertyValue<string>("Attr"),
                Children = t.Children.Where(c => c.HeaderName == "Node").Select(selector)
            });

            var nodes = x.Children.Where(c => c.HeaderName == "Node").Select(selector);

            Assert.AreEqual(2, nodes.Count());
            Assert.AreEqual("3", nodes.ElementAt(1).Children.ElementAt(0).Id);

        }


    }
}
