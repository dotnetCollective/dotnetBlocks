
using Docfx.Dotnet;
using dotNetBlocks.Docs.Toc;
using Markdig;
using Markdig.Syntax;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace dotNetBlocks.Docs
{

    /// <remarks>
    /// https://www.statiq.dev/
    /// 
    /// </remarks>
    public class Program
    {
        private static readonly string[] DefaultSourceFiles = new[]
                {
            "../src/**/{!.git,!bin,!obj,!packages,!*.Tests,}/**/*.cs", // Alongside the input folder
            "../../src/**/{!.git,!bin,!obj,!packages,!*.Tests,}/**/*.cs" // Alongside the parent project folder
        };

        /// <summary>
        /// Main exe entry point.
        /// </summary>
        /// <param Name="args">The arguments.</param>
        /// <returns></returns>
        public static async Task<int> Main(string[] args)
        {
            return await Generate();
        }

        private static async Task<int> Generate()
        {
            const string configFileName = "docfx.json";

            bool updateToc = true;
            bool generate = false;
            bool build = true;
            bool host = true;


            var repoRoot = FindRepoRoot();
            var rootFolderName = Path.Combine(repoRoot, "Docs");
            var configFile = Path.Combine(rootFolderName, configFileName);
            var sitePath = Path.Combine(repoRoot, "docs_site");

            // Ensure consistent date handling - set culture.

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            if (updateToc)
            {
                var tocGenerator = new TocGenerator();


                var rootToc = await tocGenerator.BuildTocAsync(
                    rootFolder: rootFolderName,
                    ( string? folderName, out string? title) =>
                    {
                        title = null;
                        var action = TocActions.Process | TocActions.WriteToc | TocActions.Overwrite;

                        if (folderName!.EndsWith("docs", StringComparison.OrdinalIgnoreCase))
                        {
                            title = "Documentation";
                            return TocActions.Process;
                        }
                        if (folderName.EndsWith("api", StringComparison.OrdinalIgnoreCase))
                        {
                            title = "API";
                            return TocActions.Process;
                        }
                        if (folderName.EndsWith("images", StringComparison.OrdinalIgnoreCase))
                            return TocActions.Ignore;
                        if (folderName.EndsWith("templates", StringComparison.OrdinalIgnoreCase))
                            return TocActions.Ignore;


                        return action;

                    }
                   );

            }




            // Get the glob file locations right first
            var fileMatch = new Matcher();
            var searchRoot = new DirectoryInfo(Path.Combine(repoRoot,"src"));
            var folderPattern = @"**/*dotnetblocks*/bin/**/*.*";
            var csprojectPattern = @"**/*dotnetblocks*/**.csproj";
            var dllPattern = @"**/*dotnetblocks*/bin/**/dotnetblocks*.dll";

            fileMatch.AddIncludePatterns(
                new string[]
                {
                    dllPattern
                });

            var matchResult  = 
            fileMatch.Execute(
                new DirectoryInfoWrapper(searchRoot)
                );
            Console.WriteLine(matchResult.Files.Count());

            if (generate)
            {
                // Have to include the options otherwise we get a null exception.
                await Docfx.Dotnet.DotnetApiCatalog.GenerateManagedReferenceYamlFiles(configFile, new Docfx.Dotnet.DotnetApiOptions()
                {
                    IncludeApi = (symb) => SymbolIncludeState.Include,
                }
                );
            }

            if (build)
            {

                await Docfx.Docset.Build(configFile,
                    new Docfx.BuildOptions()
                    {
                        // Enable citation Markdown extension
                          ConfigureMarkdig = pipeline => pipeline.UseGenericAttributes().UseCitations(),
                    }
                    );
            }

            if (host)
            {

                var builder = WebApplication.CreateBuilder(new WebApplicationOptions
                {
                    WebRootPath = sitePath
                });
                var app = builder.Build();

                var defaultFileOptions = new DefaultFilesOptions();
                defaultFileOptions.DefaultFileNames.Clear(); // Clear the default list
                defaultFileOptions.DefaultFileNames.Add("index.html"); // Add your custom default 
                                                                       // Order is important
                app.UseDefaultFiles(defaultFileOptions);
                app.UseStaticFiles();

                await app.RunAsync();
            }

            return 0;

        }

        /// <summary>
        /// Finds the repo root.
        /// </summary>
        /// <param Name="path">The path.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Could not find the root of the dot net blocks repo.</exception>
        static string FindRepoRoot(string? path = default)
        {
            // Start the search at the executing directory.
            var appPath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/";

            ;
            var folder = new DirectoryInfo(appPath); // Load the executing folder.
            bool found = folder!.EnumerateFiles("Directory.Build.props").Any(); // Search for the props file in the current folder.
            while (!found)
            {
                folder = folder.Parent; // move up to the parent folder
                found = folder!.EnumerateFiles("Directory.Build.props").Any(); // search for the props file.
            }

            string repoRoot = $"{folder.FullName}";
            if (!folder.FullName!.EndsWith("dotNetBlocks")) throw new Exception("Could not find the root of the dot net blocks repo.");

            return repoRoot;
        }
    }
}