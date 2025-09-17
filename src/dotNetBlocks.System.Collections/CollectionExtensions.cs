using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    /// <summary>
    /// Extends functionality of existing items types in the System.Collections.Generic namespace.
    /// </summary>
    public static class CollectionExtensions
    {

        /// <summary>
        /// Checks the enumerator for default items and throws an exception if any are found.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="exceptionFactory">Creates execption to throw when default item detected.</param>
        /// <returns>
        ///   <see cref="IEnumerable{T}" /> that doesn't allow default values.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">default(TItem) detetected</exception>
        /// <exception cref="ArgumentNullException">default(TItem) detetected</exception>
        public static IEnumerable<TItem> ThrowIfItemIsDefaultValue<TItem>(this IEnumerable<TItem> items, Func<Exception>? exceptionFactory = default)
        {

            exceptionFactory ??= () => new ArgumentNullException("default(TItem) detetected");

            // if there are no items to process then return.
            if (items == default)
                yield break; // nothing to do.

            EqualityComparer<TItem>? comparer = default; // compares items

            foreach (TItem item in items) // check each item.
            {

                comparer ??= EqualityComparer<TItem>.Default; // late initialize comparer.

                if (comparer.Equals(item, default(TItem)))
                    throw exceptionFactory();

                yield return item;

            }
        }

        /// <summary>
        /// Ignores default items in the enumeration.
        /// </summary>
        /// <typeparam name="TItem">Type of items in the enumeration.</typeparam>
        /// <param name="items">The items.</param>
        /// <returns><see cref="IEnumerable{TItem} with default items filtered out."/> </returns>
        public static IEnumerable<TItem> IgnoreDefaultValues<TItem>(this IEnumerable<TItem> items)
        {
            if (items == default)
                yield break;

            EqualityComparer<TItem>? comparer = default; // compares items

            foreach (TItem item in items)
            {

                comparer ??= EqualityComparer<TItem>.Default; // late initialize comparer.

                if (comparer.Equals(item, default(TItem)))
                    continue;

                yield return item; // yield non-default item.

            }
        }

        /// <summary>
        /// Checks that the <see cref="IQueryable{T}" /> has one or more items; otherwise throws a custom exception.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="exceptionFactory">The exception factory.</param>
        /// <returns></returns>
        /// <seealso cref="MustHaveOneOrMore{TItem}(IEnumerable{TItem}, Func{Exception})"/>  "/>
        public static IEnumerable<TItem> MustHaveOneOrMore<TItem>(this IQueryable<TItem> queryable, Func<Exception>? exceptionFactory = default) =>
        queryable.AsEnumerable<TItem>().MustHaveOneOrMore(exceptionFactory);


        /// <summary>
        /// Throws a custom exception if the <see cref=" IEnumerable{TItem}" /> has no items."/&gt; otherwise returns the items.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="exceptionFactory">called to create the exception thrown if there are no items in the enumerator.</param>
        /// <returns>
        ///   <see cref="IEnumerable{TItem} of one or more items" />
        /// </returns>
        /// <remarks>
        /// Useful to ensure that a items has at least one item before proceeding. without materializing the enumeration. Cheaper than the Linq Any() method.
        /// Manages the enumerator directly for efficiency and to avoid multiple enumeration of the items.
        /// </remarks>
        public static IEnumerable<TItem> MustHaveOneOrMore<TItem>(this IEnumerable<TItem> items, Func<Exception>? exceptionFactory = default)
        {

            exceptionFactory ??= () => new InvalidOperationException("No items in the enumeration.");

            // Test the enumerator directly to avoid multiple enumeration of the items.
            using (var it = items.GetEnumerator()) // Enumerator is IDisposable.
            {
                // Position to the first item.
                if (!it.MoveNext()) throw exceptionFactory(); // No items so build and throw exception.
                do
                {
                    yield return it.Current;
                }
                while (it.MoveNext());// Return the remaining items.
            }
        }
    }
}