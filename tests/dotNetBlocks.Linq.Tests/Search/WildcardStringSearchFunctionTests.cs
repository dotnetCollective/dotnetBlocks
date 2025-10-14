using dotNetBlocks.Linq.Tests.Search;
using LinqKit;
using LinqKit.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

[TestCategory("WildcardStringSearchFunctions")]
[TestClass]
public class WildcardStringSearchFunctionTests
{
    [TestMethod]
    public void wildcard_search__function_generation()
    {
        // starts with
        var wildcard = "a*".SearchFunc();
        wildcard("a").ShouldBeTrue();
        wildcard("b").ShouldBeFalse();
        wildcard("abc").ShouldBeTrue();

        // ends with
        wildcard = "*a".SearchFunc();
        wildcard("a").ShouldBeTrue();
        wildcard("ab").ShouldBeFalse();
        wildcard("aba").ShouldBeTrue();

        // contains
        wildcard = "*a*".SearchFunc();
        wildcard("a").ShouldBeTrue();
        wildcard("b").ShouldBeFalse();
        wildcard("bac").ShouldBeTrue();
        wildcard("bc").ShouldBeFalse();
        wildcard(null).ShouldBeFalse();

        // null testing

        // Test null wildcard
        wildcard = "abssdf".SearchFunc();
        wildcard(null).ShouldBeFalse();

        // Create a wildcard using a null value.
        wildcard = WildCard.SearchFunc(null);
        wildcard(null).ShouldBeTrue(); // null and null.
        wildcard("rttrt").ShouldBeFalse(); // its not null.

    }

}
