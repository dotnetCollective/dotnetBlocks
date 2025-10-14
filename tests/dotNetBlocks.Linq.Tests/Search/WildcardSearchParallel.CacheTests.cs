using dotNetBlocks.Linq.Tests.Search;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


[TestCategory("WildcardSearch")]
public class WildcardSearchParallelCacheTests : QueryBuilderTestsBase
{

    [TestMethod]
    public void Parallel_WildcardSearch_ensures_cache_threading()
    {
        var testChoice = new Random();

        // Arrange
        var tests = new List<Task>();
        for (int testCount = 0; testCount < 200; testCount++)
            tests.Add(
            testChoice.Next(3) switch 
            {
                0 => Task.Run( new WildCardSearchNamedFieldTests().wildcard_search__match_field1_string_and_field2_string),
                1 => Task.Run(new WildCardSearchNamedFieldTests().wildcard_search__match_field1_string_null_field2_string_null_string),
                2 => Task.Run(new WildCardSearchNestedFieldTests().wildcard_search__match_child_field2),
                _ => Task.Run(new WildCardSearchNestedFieldTests().WildCard_Search_match_child_field1_empty_string)
            }
            );

        // Act
        Task.WaitAll(tests.ToArray());

        // Assess
    }

}
