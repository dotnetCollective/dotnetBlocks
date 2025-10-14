using dotNetBlocks.Linq.Tests.Search;
using LinqKit;
using LinqKit.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

[TestCategory("WildcardSearch")]
[TestClass]
public class WildCardSyntaxPermutationTests : QueryBuilderTestsBase
{

    [TestMethod]
    public void string_compiled_function_extensions_used_in_where_query_syntax()
    {
        var results =
        from i in Fixture.Db.SearchItems
        where "*h*".Search(i.Field1) // Where clause with non object aware search.
        select i;
        results.Count().ShouldBe(6);
    }

    [TestMethod]
    public void _fluent_search_notation_using_where_function()
    {
        var results =
        from i in Fixture.Db.SearchItems
        .Where(WildCard.Search<SearchItem>().In.Property(f => f.Field1).ForValue("*h*"))
        select i;
        results.Count().ShouldBe(6);
    }

    [TestMethod]
    public void _fluent_search_notation_with_field_by_name_using_where_function()
    {
        var results =
        from i in Fixture.Db.SearchItems
        .Where(WildCard.Search<SearchItem>().In.Properties.Named("Field1").For.Value("*h*"))
        select i;
        results.Count().ShouldBe(6);
    }

}