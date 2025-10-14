using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using dotNetBlocks.Linq.Tests.Search;

[TestCategory("WildcardSearch")]

[TestClass]
public class WildCardSearchNamedFieldTests : QueryBuilderTestsBase
{

    [TestMethod]
    public void wildcard_search__match_field1_string()
    {
        // Arrange

        // Act
        var resultsStartsWith
            = Fixture.Db.SearchItems
                .Where(
                    WildCard.Search<SearchItem>().In.Properties.Named("Field1").For.Value("h*")
                    );
        var resultsEndsWith
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Properties.Named("Field1").For.Value("*h")
                );
        var resultsContains
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Properties.Named("Field1").For.Value("*h*")
                );

        // Assess
        resultsStartsWith.Count().ShouldBe(2);
        resultsEndsWith.Count().ShouldBe(2);
        resultsContains.Count().ShouldBe(6);
    }

    [TestMethod]
    public void wildcard_search__match_field2_string()
    {
        // Arrange

        // Act
        var resultsStartsWith
            = Fixture.Db.SearchItems
                .Where(
                    WildCard.Search<SearchItem>().In.Properties.Named("Field2").For.Value("h*")
                    );
        var resultsEndsWith
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Properties.Named("Field2").For.Value("*h")
                );
        var resultsContains
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Properties.Named("Field2").For.Value("*h*")
                );

        // Assess
        resultsStartsWith.Count().ShouldBe(2);
        resultsEndsWith.Count().ShouldBe(2);
        resultsContains.Count().ShouldBe(6);
    }

    [TestMethod]
    public void wildcard_search__match_field1_string_and_field2_string()
    {
        // Arrange

        // Act
        var resultsStartsWith
            = Fixture.Db.SearchItems
                .Where(
                    WildCard.Search<SearchItem>().In.Properties.Named("Field1", "Field2").For.Value("h*")
                    );
        var resultsEndsWith
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Properties.Named("Field1", "Field2").For.Value("*h")
                );
        var resultsContains
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Properties.Named("Field1", "Field2").For.Value("*h*")
                );

        // Assess
        resultsStartsWith.Count().ShouldBe(4);
        resultsEndsWith.Count().ShouldBe(4);
        resultsContains.Count().ShouldBe(12);
    }

    [TestMethod]
    public void wildcard_search__match_field1_string_empty_string()
    {
        // Arrange

        // Act
        var resultsAll
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Properties.Named("Field1").For.Value(string.Empty)
                );

        // Assess
        resultsAll.Count().ShouldBe(17);
    }

    [TestMethod]
    public void wildcard_search__match_field1_string_null_string()
    {
        // Arrange

        // Act
        var resultsNull
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Properties.Named("Field1").For.Value(null)
                );

        // Assess
        resultsNull.Count().ShouldBe(2);
    }

    [TestMethod]
    public void wildcard_search__match_field2_string_null_string()
    {
        // Arrange

        // Act
        var resultsNull
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Properties.Named("Field2").For.Value(null)
                );

        // Assess
        resultsNull.Count().ShouldBe(2);
    }

    [TestMethod]
    public void wildcard_search__match_field1_string_null_field2_string_null_string()
    {
        // Arrange

        // Act
        var resultsNull
            = Fixture.Db.SearchItems
            .Where(
                WildCard.Search<SearchItem>().In.Properties.Named("Field1", "Field2").For.Value(null)
                );

        // Assess
        resultsNull.Count().ShouldBe(3);
    }
}
