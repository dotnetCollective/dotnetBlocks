using System;
using System.Buffers;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace System.Linq;

/// <summary>
/// Wild Card Root and extensions
/// </summary>
/// <example>
/// <code>
/// WildCard.New<SearchItem>().In
/// </code>
/// </example>
public static class WildCard
{


    #region Basic optimized wildcard searches.

    /// <summary>
    /// Extension for searching using a wildcar stringin another string.
    /// </summary>
    /// <code>
    /// 
    /// WildcardText.Search(targetText);
    /// 
    /// from i in DB
    /// where textbox.value.Search(i.Field1)
    /// select i;
    /// </code>
    /// <remarks>
    /// Use THIS method got Linq query syntax
    /// Use this method when you are starting with a string or a string field
    /// </remarks>
    ///
    /// <param name="wildcardSearchFor">The wildcard string search for.</param>
    /// <param name="searchIn"> String to search in for the wildcard.</param>
    /// <returns> <see langword="true"/> if <see wildcard matches string. </returns>
    public static bool Search(this string? wildcardSearchFor, string? searchIn) => wildcardSearchFor.SearchFunc()(searchIn);

    /// <summary>
    /// Creates function to search for the wildcard in any string.
    /// </summary>
    /// <code>
    /// 
    /// WildcardText.SearchFunc(targetText);
    /// 
    /// from i in DB
    /// where textbox.value.SearchFunc()(i.Field1)
    /// select i;
    /// </code>
    /// <remarks>
    /// Use THIS method got Linq query syntax
    /// Use this method when you are starting with a string or a string field
    /// </remarks>
    ///
    /// <param name="wildcardSearchFor">The wildcard string search for.</param>
    /// <returns> <see cref="Func{<see cref="string?"/>, <see cref="bool"/>}"/> function that executes an optimized wildcard search on a provided string. </returns>
    public static Func<string?, bool> SearchFunc(this string? wildcardSearchFor) => SearchExpressionBuilder.BuildWildcardSearchExpression(wildcardSearchFor).Compile();


    #endregion Basic optimized wildcard searches.

    #region IQueryable, IEnumerable
    public static IQueryable<TSearch> Search<TSearch>(this IQueryable<TSearch> query, Action<WildCard<TSearch>> configure)
        where TSearch : class
    {
        var search = WildCard<TSearch>.New;
        search.In.Queryable = query;
        configure(search);

        // Apply the search to the queryable.
        query = query.Where(search.Where);

        // Fluent syntax
        return query;

    }

    public static IEnumerable<TSearch> Search<TSearch>(this IEnumerable<TSearch> query, Action<WildCard<TSearch>> configure)
        where TSearch : class
    {
        var search = WildCard<TSearch>.New;
        search.In.Enumerable = query;

        configure(search);

        // Apply the search to the enumerable
        query = query.Where(search.Where);

        // Fluent syntax
        return query;

    }

    #endregion IQueryable, IEnumerable



    /// <summary>
    /// Fluent Wildcard search statement
    /// </summary>
    /// <code>
    /// Where(WildCard.SearchExp.In.Property().ForValue());
    /// WildCard.
    /// </code>
    /// <typeparam name="TSearch">The type of the search.</typeparam>
    /// <returns></returns>
    public static WildCard<TSearch> Search<TSearch>()
        where TSearch : class
        => WildCard<TSearch>.New;


    /// <summary>
    /// Extends a wildcard string for a wildcard search and 
    /// Returns Delegate used in Linq Where statement.
    /// </summary>
    /// <typeparam name="TSearch">The type of the search.</typeparam>
    /// <param name="searchString">The search string.</param>
    /// <param name="properties">The properties.</param>
    /// <returns><see cref="WildCard<TSearch>.Target.SearchWhere"/> used in search clause.</returns>
    /// <code>
    /// db.Items.Where( w => "*abd".SearchExp<Titem>( p => p.Field1)).Select(i);
    /// </code>
    public static WildCard<TSearch>.Target.SearchWhere Search<TSearch>(this string? searchString, params Expression<Func<TSearch, string?>>[] properties)
        where TSearch : class
    {
        var search = WildCard<TSearch>.New;
        search.In.Property(properties);
        search.For.Value(searchString);
        return search.Where;
    }


    /// <summary>
    /// Wildcard search capturing instance of the extended item
    /// </summary>
    /// <typeparam name="TSearch">The type of the search.</typeparam>
    /// <param name="_">The .</param>
    /// <returns></returns>
    /// <remarks>
    /// <code>
    /// from i in db.Items where i.New().ForValue("sdf*"). /// implicitly Converted to <see cref="bool"/>
    /// </code>
    /// Used for implicit conversion of the Where statement to <see cref="Func{TSearch, bool}<"/>
    /// Used from Linq Query Syntax where operator that reuquired a <see langword="bool"/> result.
    /// </remarks>
    public static WildCard<TSearch> Search<TSearch>(this TSearch item)
        where TSearch : class

     { var search =  WildCard<TSearch>.New;
        search.In.Item = item;
        return search;
    }




}

public class WildCard<TSearch>
{


    public static WildCard<TSearch> New => new WildCard<TSearch>();

    public struct PropertyAccessor
    {
        public PropertyInfo FieldInfo;
        public MemberExpression Accessor;
        public string Key => FieldInfo.Name;
    }

    // Build field accessors
    private static readonly ParameterExpression _searchTarget = Expression.Parameter(typeof(TSearch), "searchTarget");

    // All properties accessing string values.
    // Used when specifying search fields by name.
    protected static IDictionary<string, PropertyInfo> validProperties = (from p in typeof(TSearch).GetProperties()
                                                                          where typeof(string).IsAssignableFrom(p.PropertyType)
                                                                          select p).ToDictionary(p => p.Name);

    protected readonly static IDictionary<string, PropertyAccessor> validfieldAccessors;


    static WildCard()
    {
        // Initialize the reflection information and expressions for valid members.

        var searchTarget = Expression.Parameter(typeof(TSearch), "searchTarget");

        validfieldAccessors =
            (from p in typeof(TSearch).GetProperties()
             where typeof(string).IsAssignableFrom(p.PropertyType)
             select new PropertyAccessor()
             {
                 FieldInfo = p,
                 Accessor = Expression.PropertyOrField(_searchTarget, p.Name)
             }).ToDictionary(p => p.Key);

    }

    public WildCard()
    {
        _target = new Target(this);
        _for = new Target.SearchFor(this);
        _where = new Target.SearchWhere(this);

    }


    private readonly Target _target;
    private readonly Target.SearchFor _for;
    private readonly Target.SearchWhere _where;

    public Target.SearchFor For => _for;
    public Target In => _target;

    public Target.SearchWhere Where => _where;

    public class Term()
    {
        public required  WildCard<TSearch> Search { get; init; }

        [SetsRequiredMembers]
        public Term (WildCard<TSearch> search) : this()
        {
            Search = search;

        }

    }

    public class Target: Term
    {
        [SetsRequiredMembers]
        public Target(WildCard<TSearch> search) : base(search) { }

        internal protected Dictionary<string, PropertyAccessor> SelectedProperties { get; } = new();
        internal protected Dictionary<string, MemberExpression> nestedProperties { get; } = new();

        // Pass through targets for use in where clause (especially for implicit conversion.
        internal protected IEnumerable<TSearch>? Enumerable { get; set; } = default;
        internal protected IQueryable<TSearch>? Queryable { get; set; } = default;
        internal protected TSearch? Item { get; set; } = default;

        public SearchFor Property(Expression<Func<TSearch, string?>> propertySelector) => AddProperty(propertySelector);
        public SearchFor Property(params Expression<Func<TSearch, string?>>[] propertySelector)
        {
            foreach (var selector in propertySelector) AddProperty(selector);
            return Search.For;
        }

        private SearchFor AddProperty(Expression<Func<TSearch, string?>> selector)
        {
            Search.Where.Reset();

            // Add the member expression.

            //get " class.property =  " accessor
            var selectorexpression = getAccessorExpression(selector);

            if (selectorexpression.Member.DeclaringType != typeof(TSearch)) // nested property declaration so we use the expression as is
            {
                if (nestedProperties.ContainsKey(selectorexpression.Member.Name))
                    return Search.For; // already added 
                else
                {
                    nestedProperties.Add(selectorexpression.Member.Name, selectorexpression);
                    return Search.For;
                }
            }

            //  Already selected?
            if (SelectedProperties.ContainsKey(selectorexpression.Member.Name))
                return Search.For;


            // Check property is valid.
            if (!validfieldAccessors.TryGetValue(selectorexpression.Member.Name, out var accessor))
                throw new ArgumentOutOfRangeException(selectorexpression.Member.Name, $"{selectorexpression.Member.Name} is not a valid string property");

            // Add to selected properties.
            SelectedProperties.Add(accessor.Key, accessor);

            return Search.For;
        }


        public NamedTarget Properties => new NamedTarget(Search);


        /// <summary>
        /// Gets the field accessor expression from the propertyselector selector
        /// </summary>
        /// <param name="propertySelector">Expression selecting the propertyselector from the class</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Property acessors must specify search field e.g. x => x.Name</exception>
        protected MemberExpression getAccessorExpression(Expression<Func<TSearch, string?>> propertySelector)
            // field.Body type = MemberAccess, Expression is the parent. Walk the stack.
            // Body = PropertyExpression -> Member and Expression
            => propertySelector.Body as MemberExpression ?? throw new ArgumentNullException("Property acessors must specify search field e.g. x => x.Name");

        /// <summary>
        /// Generates and Caches the wildcard search expression used in the where cluase.
        /// </summary>
        public class SearchWhere: Term
        {
            [SetsRequiredMembers]
            public SearchWhere(WildCard<TSearch> search) : base(search) { }

            // Conversions to help the where clause seamlessly be useful in queries.

            public static implicit operator Expression<Func<TSearch, bool>>(SearchWhere where) => where.WhereExp;

            public static implicit operator Func<TSearch, bool>(SearchWhere where) => where.WhereFunc;

            public void InEnumerable(IEnumerable<TSearch>? items) { Search.In.Enumerable = items; }
            public IEnumerable<TSearch>? Enumerable => Search.In.Enumerable;
            public void InQueryable(IQueryable<TSearch>? items) { Search.In.Queryable = items; }
            public IQueryable<TSearch>? Queryable => Search.In.Queryable;

            /// <summary>
            /// Resets the search command 
            /// </summary>
            protected internal void Reset()
            {
                _whereExp = null;
                _whereFunc = null;
            }

            private Expression<Func<TSearch, bool>>? _whereExp = default;

            [NotNull]
            public Expression<Func<TSearch, bool>> WhereExp => 
                _whereExp ??= SearchExpressionBuilder.BuildSearchExpression<TSearch>(Search.In.SelectedProperties.Values.Select(p => p.Accessor).Union(Search.In.nestedProperties.Values), Search.For.SearchValue);

            private Func<TSearch, bool>? _whereFunc = default;

            [NotNull]
            private Func<TSearch, bool>? WhereFunc => _whereFunc ??= WhereExp.Compile();

            [NotNull]
            public Expression<Func<TSearch, bool>> Where => WhereExp;
            public bool WhereValue(TSearch value) => WhereFunc(value);


        }

        public class SearchFor : Term
        {
            [SetsRequiredMembers]
            public SearchFor(WildCard<TSearch> Search) : base(Search) { }

            /// <summary>
            /// New ValueWhere after analysis.
            /// </summary>
            [NotNull]
            internal protected SearchStringAnalysisResult? SearchValue = new(default);

            public SearchWhere Value(string? searchValue) => ForValue(searchValue);

            public SearchWhere ForValue(string? searchValue)
            {
                SearchValue = SearchStringAnalyzer.AnalyzeSearchString(searchValue);
                Search.Where.Reset(); // Clear the cached search expression and function

                return Search.Where!; // return the where clause.
            }

        }

            #region  Named Target Target Properties By Name


            public class NamedTarget : Term
        {

            [SetsRequiredMembers]
            public NamedTarget(WildCard<TSearch> Search) : base(Search) { }

            public NamedTarget AddAll()
            {
                var addNames = validfieldAccessors.Keys.Except(Search.In.SelectedProperties.Keys); // All valid fields not yet selected.
                foreach (var name in addNames)
                    Search.In.SelectedProperties.Add(name, validfieldAccessors[name]);

                return this;
            }


            public NamedTarget Named(params string[] names)
            {
                ArgumentOutOfRangeException.ThrowIfZero(names.Length);

                ValidateFields(names);

                var addNames = names.Except(Search.In.SelectedProperties.Keys); // ad names not already selected.

                foreach (var name in addNames)
                    Search.In.SelectedProperties.Add(name, validfieldAccessors[name]); // add selector

                return this;
            }

            public NamedTarget ButNamed(params string[] names)
            {
                ArgumentOutOfRangeException.ThrowIfZero(names.Length);

                ValidateFields(names);

                var removeNames = names.Intersect(Search.In.SelectedProperties.Keys); // names already selected

                foreach (var name in removeNames)
                    Search.In.SelectedProperties.Remove(name);

                return this;
            }



            public NamedTarget NotNamed(params string[] names)
            {
                ArgumentOutOfRangeException.ThrowIfZero(names.Length);

                checkFieldNameNotation(names);  // Ensure field named properly.

                var addNames = validfieldAccessors.Keys.Except(names).Except(Search.In.SelectedProperties.Keys); // valid names except not name except already selected.
                foreach (var name in addNames) // select the new properties
                    Search.In.SelectedProperties.Add(name, validfieldAccessors[name]);

                return this;
            }

            public NamedTarget ButNotNamed(params string[] names)
            {
                ArgumentOutOfRangeException.ThrowIfZero(names.Length);

                checkFieldNameNotation(names);  // Ensure field named properly.

                var removeNames = Search.In.SelectedProperties.Keys.Intersect(names); // Remove all these selectors

                foreach (var name in removeNames) // remove properties
                    Search.In.SelectedProperties.Remove(name);

                return this;
            }



            public NamedTarget AllProperties => this;

                #endregion Named Target Target Properties By Name


            public SearchFor For => Search.For;


            private void checkFieldNameNotation(IEnumerable<string> fieldNames)
            {
                if (fieldNames.Any(fn => fn.Contains('.')))
                    throw new ArgumentException(nameof(fieldNames), "'.' child propertyselector notation is only  supported using Lambda specification methods ");
            }

            private void ValidateFields(IEnumerable<string> fieldNames)
            {
                // Ensure field names are valid format.
                checkFieldNameNotation(fieldNames);

                foreach (var name in fieldNames)
                {
                    if (!validfieldAccessors.ContainsKey(name))
                        throw new ArgumentException($" {name} is not a valid property field name ");
                }
            }
        }
    }
}