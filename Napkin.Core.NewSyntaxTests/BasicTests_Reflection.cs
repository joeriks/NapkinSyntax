using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

namespace Napkin.NewSyntaxTests
{
    [TestClass]
    public class BasicTest_Reflection
    {

        [TestMethod]
        public void ShortSyntax_Indentation_Reflection()
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


            Func<Node, SomeNode> creator = null;
            var createChildren = new Action<Node, SomeNode>((n, s) =>
             {
                 s.Children = n.Children.Select(creator);
             });
            creator = new Func<Node, SomeNode>((s) =>
             {
                 return s.CreateInstance(createChildren);
             });


            var nodes = x.Children.Where(c => c.HeaderName == "Node").Select(creator);

            Assert.AreEqual(2, nodes.Count());
            Assert.AreEqual("3", nodes.ElementAt(1).Children.ElementAt(0).Id);

        }


        [TestMethod]
        public void WithAttributes_Indentation_Linq_Recursive()
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


            var nodes = x.Children.Where(c => c.HeaderName == "Node").Select(t=>t.CreateInstance<SomeNode>());

            Assert.AreEqual(2, nodes.Count());
            Assert.AreEqual("3", nodes.ElementAt(1).Children.ElementAt(0).Id);


        }


        public class Item
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public IEnumerable<Item> Children { get; set; }
        }

        [TestMethod]
        public void WithAttributes_Short()
        {

            var napkinDocument = @"
                                    Item Id=1
                                        Name=Foo
                                        Description=Fuu

                                    Item Id=2
                                        Name=Bar

                                        Item Id=201
                                            Name=Baz

                                    Item
                                        Id=3
                                        Name=Bax";

            var items = napkinDocument.Deserialize<Item>();
            Assert.AreEqual(3, items.Count());

            var itemWithId2 = items.SingleOrDefault(t => t.Id == 2);
            Assert.AreEqual(2, itemWithId2.Id);

            Assert.AreEqual(201, itemWithId2.Children.SingleOrDefault().Id);
            Assert.AreEqual("Bax", items.ElementAt(2).Name);

        }



    }
}
