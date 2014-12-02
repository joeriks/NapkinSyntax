##Napkin Syntax parser and deserializer

The "Napkin Syntax" is my version of an "as simple as possible human readable object notation", where a document is described
with nodes in new lines, and their properties indented one level. It also supports hierarchies of items.

The syntax parser creates a napkin node tree, which can be used directly with linq to create objects, or use the build in small 
reflection based magic instantiator (see below).

**Why?**
Because no one likes to create object trees with more syntax than necessary.

**How big?**
~300 Locs

**Portable?**
Yes

**Test coverage?**
Some

**Try it with the WPF-app with built in ScriptCs transformation scripting**

![WPF](/Documentation/wpfimage.PNG)

**Status?**
Long time idea that needed to be materialized. Not used in production just yet.

	[Item type name] [Attributes...]
		[PropertyX=Value]
		[...]

	[Item type name] [Attributes...]
		[Properties]
		[...]

		[Sub item type name] [Attributes...]
			[Properties]
			[...]

	[...]

Attributes are merged into Properties but might be handled separately if needed.

Example using reflection to instantiate to objects:

    using Napkin;
	
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


Example of explicit syntax (with recursive selects):

	var napkinDocument = new Node(@"
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

    var nodes = napkinDocument.Children.Where(c => c.HeaderName == "Node").Select(nodeSelector);

    Assert.AreEqual(2, nodes.Count());
    Assert.AreEqual("3", nodes.ElementAt(1).Children.ElementAt(0).Id);
