## dotnetBlocks.Linq

part of the [.netBlocks Project](https://dotnetblocks.dotnetcollective.org/) - https://dotnetblocks.dotnetcollective.org/

### Description

Adds optimized wildcard search capabilities when searching properties using Linq. extends System.Linq namespace.


- 



[licensing, usages, design, other details](https://dotnetblocks.dotnetcollective.org/)

# getting started

* standard nuget package.


# licensing, usages, design and other details.


[main project site] (https://dotnetblocks.dotnetcollective.org/)

## Examples

Details usage notation in the docs.

``` csharp
// Linq Query syntax
var results = 
	from i in DB.Items
	where textBox.Value.Search(i.Field1)
select i;

// LinqMethod Syntax

var results = 
	from i in DB.Items
	.Where( i.search( ws => ws.In.Property(f => f.field1).ForValue(textBox.Value))
	select i;

```