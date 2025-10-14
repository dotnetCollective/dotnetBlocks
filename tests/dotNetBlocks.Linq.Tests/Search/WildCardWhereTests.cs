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
public class WildCardSearchLambdaTests : QueryBuilderTestsBase
{
    [TestMethod]
    public void wildcard_search__match_field1()
    {

        // Arrange

        // Act
        var resultsStartsWith
            = Fixture.Db.SearchItems
                .Where(
                    WildCard.Search<SearchItem>().In.Property(i => i.Field1).ForValue("h*")
                    );
        var resultsEndsWith
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Field1).ForValue("*h")
                );
        var resultsContains
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Field1).ForValue("*h*")
                );

        // Assess
        resultsStartsWith.Count().ShouldBe(2);
        resultsEndsWith.Count().ShouldBe(2);
        resultsContains.Count().ShouldBe(6);
    }

    [TestMethod]
    public void wildcard_search__match_field2()
    {
        // Arrange

        // Act
        var resultsStartsWith
            = Fixture.Db.SearchItems
                .Where(
                    WildCard.Search<SearchItem>().In.Property(i => i.Field2).ForValue("h*")
                    );
        var resultsEndsWith
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Field2).ForValue("*h")
                );
        var resultsContains
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Field2).ForValue("*h*")
                );

        // Assess
        resultsStartsWith.Count().ShouldBe(2);
        resultsEndsWith.Count().ShouldBe(2);
        resultsContains.Count().ShouldBe(6);
    }

    [TestMethod]
    public void wildcard_search__match_field1_and_field2()
    {
        // Arrange

        // Act
        var resultsStartsWith
            = Fixture.Db.SearchItems
                .Where(
                    WildCard.Search<SearchItem>().In.Property(i => i.Field1, i => i.Field2).ForValue("h*")
                    );
        var resultsEndsWith
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Field1, i => i.Field2).ForValue("*h")
                );
        var resultsContains
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Field1, i => i.Field2).ForValue("*h*")
                );

        // Assess
        lock (Fixture.ThreadLock)
        {
            resultsStartsWith.Count().ShouldBe(4);
            resultsEndsWith.Count().ShouldBe(4);
            resultsContains.Count().ShouldBe(12);
        }
    }

    [TestMethod]
    public void wildcard_search__match_field1_empty_string()
    {
        // Arrange

        // Act
        var resultsAll
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Field1).ForValue(string.Empty)
                );

        // Assess
        resultsAll.Count().ShouldBe(17);
    }

    [TestMethod]
    public void wildcard_search__match_field1_null_string()
    {
        // Arrange

        // Act
        var resultsNull
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Field1).ForValue(null)
                );

        // Assess
        resultsNull.Count().ShouldBe(2);
    }

    [TestMethod]
    public void wildcard_search__match_field2_null_string()
    {
        // Arrange

        // Act
        var resultsNull
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Field2).ForValue(null)
                );

        // Assess
        lock (Fixture.ThreadLock)
            resultsNull.Count().ShouldBe(2);
    }

    [TestMethod]
    public void wildcard_search__match_field1_null_field2_null_string()
    {
        // Arrange

        // Act
        var resultsNull
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Field1, i => i.Field2).ForValue(null)
                );

        // Assess
        resultsNull.Count().ShouldBe(3);
    }
}
