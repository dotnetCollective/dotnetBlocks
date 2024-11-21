using dotNetBlocks.docs;

namespace TestProject1dotNetBlocks.docs.Tests
{
    [TestClass]
    public sealed class DocGenRunner
    {
        [TestMethod]


        
        public async Task RunMain()
        {
            //var args = new string[] { "--help" };
            var args = new string[] { @"-- input", @"C:\Code\Repos\dotNetCollective\dotNetBlocks", "-l","debug" };

            await Program.Main(args);
        }
    }
}
