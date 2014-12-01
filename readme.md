##Napkin Syntax deserializer

The "Napkin Syntax" is my version of an "as simple as possible human readable object notation", where a document is described
with nodes in new lines, and their properties indented one level. It also supports hierarchies of items.

**Why?**
Because no one likes to create object trees with more syntax than necessary.

**How big?**
~300 Locs

**Portable?**
Yes

**Test coverage?**
Some

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


Example from one of the tests:

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