# Wildcard Search extensions

## Objective

## Problem solved

Users want to search fields for text in various positions but "like" implements "contains" is slow and taxing on database systems.
This class supports wildcard string searching in the most effecient, but not completely efficient method based on the user input.

## Solution
The class Examines the user provided strings for wildcards and builds a search predicate
for the most optimal search delegate. Supports "*" and "%" wildcards.

## How we optimized the search

* *null* or No Wildcard "abc" => Equals
* Wildcard start "*abc" or "%abc" => EndsWith
* Wildcard end "abc*" or "abc%" => StartsWith
* Wildcard middle "a*bc" or "a%bc" => Contains
* Empty string, all wildcard string "" or "%*" => NoOp - "True" is always **True** for this term.
* Multiple properties combine with OrElse

# Usage

WildCard search supports a fluent notation and a set of string extensions.

## String Extensions

Used as compiled methods and are not object aware or property accessor aware.
Simplest form of search.


Used for linq fluent syntax and string searches. Can use in linq method syntax where clause.


Search() returns a function f(string?, bool) returning *true* for a wildcard match and *false* if no match detected.
"wildcard".Search("searchin") - compiled function.

``` csharp
var wildcard = wildcardText.Search(); // or var wildcard = "*sdf*".Search();

bool result = wildcard("do I match);

```

``` csharp

// linq Query syntax where statement

from i in DB
where textbox.Text.Search(i.field1) select i; // Compiled string extension.

```

### Notes


### Restrictions
Named property notation (properties by name, not by lamba delegate definition, only support field names on the search object and doesn't support child "dotted" notations and will throw an exception.

### Syntax - Fluent notations for class aware search functionality.

Root .In <Target> .For <ForValue> .Where <Where> 

Root = WildCard.Search<TSearch> - specifies type of class
<Target properties to search on TSearch.
  Lambda specification OR named specification
    ->  Property(property lamda)
        ->  Properties. Named Properties
            -> Named( string[] field names)
< For Value -> Wildcard value or pattern to match.







## Tech notes

The expression builders uses MemoryCache to reduce the impact of expression building and visitations.
Property accessor members provided by the caller are cached and re-used.
Visitor class updates the target parameter and the constant used in the search.
IF you increase text support to child objects, split the string and iterate on the Expression.Property method
but you will lose inclusion / exclusion functionality.


## Linq

### How Linq queries work.

([Standard Query Operators - Linq query syntax](https://learn.microsoft.com/en-us/dotnet/csharp/linq/standard-query-operators/))

To understand the predicate output of the search and how the extensions are structured, you need to understand a little about how the Linq syntax, operators and methods work under the covers.
A lot of research was put into understanding this, but here is the gist of it.

Linq has two primary formats:
* Linq Query Syntax - built into C# supports inline query style
* Linq Method Syntax - uses fluent method notation to define the query
## Linq formats
##  Linq Query syntax

```csharp

from i in db.items
where i.field="abd"
select i;

```

## Linq Method syntax

```csharp

db.items.Where(i=> i.field="abd").Select(i);

```

## Where clause

The most important difference between these two methods also impacts the implementation of the "wildcard search" classes.

query syntax requires code resulting in a boolean value.
method syntax accepts an 
```csharp
 Expression<Func<T,bool>
```
which is passe in as a parameter to the 
````csharp
Where(Iqueryable<T>, Expression<Func<TBool>)
```
method.

## Mixing syntax

In the query syntax, when querying a DataSet<T> the select results in a IQueryable<T> that can we applied to the selected result. In the where clause, the where method can be chained
BEWARE!! of switching between IQueryable<T> and IEnumerable <T> where one is executed server side and the other client side.

Multiple where clauses are combined using "and also:

```csharp
        var q = from i in Fixture.Db.SearchItems
            .Where( i => .Field1 == "test")
            where i.Field2="sdfdsf"
            select i;

```

## Designing the wildcard search
The Wildcard search extensions are design to conform to these requirements as fluently as possible.



# Extensions signature design

The IQueryable<T> typically for database queries is contrained to class, so we contrain to class.