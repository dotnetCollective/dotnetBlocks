using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using dotNetBlocks.Linq.Tests.Search;

[TestCategory("WildcardSearch")]

[TestClass]
public class WildCardSearchNestedFieldTests : QueryBuilderTestsBase
{
    [TestMethod]
    public void wildcard_search__match_child_field1()
    {
        // Arrange

        // Act
        var startsWithResults
            = Fixture.Db.SearchItems
                .Where(
                    WildCard.Search<SearchItem>().In.Property(i => i.Child!.ChildField1).ForValue("h*")
                );
        var endsWithResults
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Child!.ChildField1).ForValue("*h")
                );
        var containsResults
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Child!.ChildField1).ForValue("*h*")
                );

        // Assess
        startsWithResults.Count().ShouldBe(2);
        endsWithResults.Count().ShouldBe(2);
        containsResults.Count().ShouldBe(6);
    }

    [TestMethod]
    public void wildcard_search__match_child_field2()
    {
        // Arrange

        // Act
        var startsWithResults
            = Fixture.Db.SearchItems
                .Where(
                    WildCard.Search<SearchItem>().In.Property(i => i.Child!.ChildField2).ForValue("h*")
                    );
        var endsWithResults
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Child!.ChildField2).ForValue("*h")
                );
        var containsResults
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Child!.ChildField2).ForValue("*h*")
                );

        // Assess
        lock (Fixture.ThreadLock)
        {
            startsWithResults.Count().ShouldBe(2);
            endsWithResults.Count().ShouldBe(2);
            containsResults.Count().ShouldBe(6);
        }
    }

    [TestMethod]
    public void wildcard_search__match_child_field1_and_child_field2()
    {
        // Arrange

        // Act
        var resultsStartsWith
            = Fixture.Db.SearchItems
                .Where(
                    WildCard.Search<SearchItem>().In.Property(i => i.Child!.ChildField1, i => i.Child!.ChildField2).ForValue("h*")
                    );
        var resultsEndsWith
            = Fixture.Db.SearchItems
            .Where(
                    WildCard.Search<SearchItem>().In.Property(i => i.Child!.ChildField1, i => i.Child!.ChildField2).ForValue("*h")
                );
        var resultsContains
            = Fixture.Db.SearchItems
            .Where(
                    WildCard.Search<SearchItem>().In.Property(i => i.Child!.ChildField1, i => i.Child!.ChildField2).ForValue("*h*")
                );

        // Assess
        resultsStartsWith.Count().ShouldBe(4);
        resultsEndsWith.Count().ShouldBe(4);
        resultsContains.Count().ShouldBe(12);
    }

    [TestMethod]
    public void WildCard_Search_match_child_field1_empty_string()
    {
        // Arrange

        // Act
        var resultsAll
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Child!.ChildField1).ForValue(string.Empty)
                );

        // Assess
        lock (Fixture.ThreadLock)
            resultsAll.Count().ShouldBe(17);
    }

    [TestMethod]
    public void Wildcard_Search_match_child_field1_null_string()
    {
        // Arrange

        // Act
        var nullmatchResults
            = Fixture.Db.SearchItems
            .Where( WildCard.Search<SearchItem>().In.Property(i => i.Child!.ChildField1).ForValue(null)
                );

        // Assess
        nullmatchResults.Count().ShouldBe(2);
    }

    [TestMethod]
    public void Wildcard_Search_match_child_field2_null_string()
    {
        // Arrange

        // Act

        var nullmatchResults
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Child!.ChildField2).ForValue(null)
                );

        // Assess
        nullmatchResults.Count().ShouldBe(2);
    }

    [TestMethod]
    public void Wildcard_Search_match_child_field1_null_field2_null_string()
    {
        // Arrange

        // Act
        var nullMatchResults
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Property(i => i.Child!.ChildField1, i => i.Child!.ChildField2).ForValue(null)
                );

        // Assess
        nullMatchResults.Count().ShouldBe(3);
    }

}
