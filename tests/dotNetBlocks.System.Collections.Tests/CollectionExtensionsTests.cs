using Shouldly;
using System.Collections.Generic;

namespace dotNetBlocks.System.Collections.Tests
{


    [TestClass]
    public class CollectionExtensionsTests
    {
        [TestMethod]
        public void ignore_default_values_should_filter_nulls()
        {
            var items = new List<int?>() { 1, 2, null, 4, null, 6 };

            items.IgnoreDefaultValues().Count().ShouldBe(4);
            items.IgnoreDefaultValues().ShouldBe(new List<int?>() { 1, 2, 4, 6 });


        }
        [TestMethod]
        public void throw_if_default_value_should_throw_when_list_contains_default_values()
        {
            var items = new List<int?>() { 1, 2, null, 4, null, 6 };

            var action = () => items.ThrowIfItemIsDefaultValue(exceptionFactory: () => new ArgumentOutOfRangeException()).ToList();

            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void must_have_one_or_more_throws_when_enumeration_is_empty()
        {
            var items = new List<int?>() { };
            var action = () => items.MustHaveOneOrMore(exceptionFactory: () => new ArgumentOutOfRangeException()).ToList();
            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        public void must_have_one_or_more_throws_when_queryable_is_empty()
        {
            var items = new List<int?>() { };
            var action = () => items.AsQueryable().MustHaveOneOrMore(exceptionFactory: () => new ArgumentOutOfRangeException()).ToList();
            action.ShouldThrow<ArgumentOutOfRangeException>();
        }
    }
}