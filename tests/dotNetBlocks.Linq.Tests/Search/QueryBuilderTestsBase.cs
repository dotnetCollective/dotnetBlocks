using dotNetBlocks.Linq.Tests.Search.Data;
using System.Diagnostics.CodeAnalysis;

// http://www.williablog.net/williablog/post/2009/12/15/Mock-a-database-repository-using-Moq.aspx 


namespace dotNetBlocks.Linq.Tests.Search;

[TestCategory("WildcardSearch")]
[TestClass]
public class QueryBuilderTestsBase

{
    private static QueryTestDataFixture? _fixture { get; set; } = default;

    public QueryTestDataFixture Fixture => _fixture!;

    [AssemblyInitialize]
    [MemberNotNull(nameof(_fixture))]
    public static void Setup(TestContext context)
    {
        _fixture = new QueryTestDataFixture();
    }

    [AssemblyCleanup]
    public static void TearDown()
    {
        _fixture!.Dispose();
        _fixture = default;
    }

    [GlobalTestInitialize]
   public static async Task TestInitialize(TestContext context)
    {
        await _fixture!.ThreadLock.WaitAsync();
    }

    [GlobalTestCleanup]
    public static void TestCleanup(TestContext context)
    {
        _fixture!.ThreadLock.Release();
    }

}