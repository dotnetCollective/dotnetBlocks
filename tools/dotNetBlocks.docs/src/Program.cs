using System;
using System.Globalization;
using System.Threading.Tasks;
//using Devlead.Statiq.Themes;
using Statiq.App;
using Statiq.Web;
using Statiq.Docs;
using dotless.Core.Parser.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using Statiq.Common;
using System.IO;
using System.Net.Http;
using System.Linq;
using Shouldly;
using NetFabric.Hyperlinq;
using Octokit;
using Statiq.Core;
using System.Reflection;
using System.Collections.Generic;
using Spectre.Console;
using System.IO.Packaging;
using Statiq.Markdown;
using dotless.Core.Loggers;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace dotNetBlocks.docs
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

        public static async Task<int> Main(string[] args)
        {

            // Ensure consistent date handling

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            // Parameters for debugging.
            //args = new string[] { "serve" };
            //args = new string[] { "-l Debug", "run serve", };

            //args = new string[] { "-- loglevel", $"{LogLevel.Warn.ToString()}" };

            //args = new string[] { "glob", "eval", @"\src/**/!.git,!bin,!obj,!packages,!*.Tests,/**/*.cs", @"C:\Code\Repos\dotNetCollective\dotNetBlocks\" };
            //args = new string[] { @"glob eval"};

            //args = new string[] { "-- preview" };
            //args = new string[] { "preview" };
            args = new string[] { "-l Debug", "-- serve" };


            // Set up the bootstrapper

            var bootstrapper = Bootstrapper.Factory.CreateDocs(args);

            // Prepare all the paths

            // This entire system is sensitive to paths relative to the filesystem root and its default intput path, which can move.
            // The root path is set by default and if you change it or use absolute paths, things break quickly, so we need to work everything relative to the default root path.
            // Use the filesystem to calculate all the relative paths.

            // Grab the bootstrapper file system to calculate the relative paths.
            var fileSystem = bootstrapper.FileSystem;

            var binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); // The actual file system binary location.
            var repoRoot = FindRepoRoot(binPath);
            //bootstrapper.SetRootPath(repoRoot);
            fileSystem.RootPath = repoRoot;

            // App path binary path relative to the statiq root path.
            var appPath = fileSystem.RootPath.GetRelativePath(binPath);

            // Prepare all the relative paths.
            var rootPath = fileSystem.RootPath.GetRelativePath(repoRoot);
            var docsPath = rootPath.Combine("docs");
            var srcPath = rootPath.Combine("src");
            var tempPath = appPath.Combine("temp");
            var cachePath = appPath.Combine("cache");
            //var outputPath = docsPath.Combine("output");
            var outputPath = docsPath.Combine("../test_output");
            var themePath = appPath.Combine("themes/docable/");

            var sourceFiles = new string[]
            {
                "../src/**/{!.git,!bin,!obj,!packages,!*.Tests,}/**/*.cs", // Alongside the input folder
                "../../src/**/{!.git,!bin,!obj,!packages,!*.Tests,}/**/*.cs" // Alongside the parent project folder
            };

            // GLOB TEST code
            // fileSystem.GetInputFiles(@"../src/{!.vs,!obj,*}/{src,!bin,!*test*,!obj,*}/{src,!*test*,,!.vs,!bin,!obj,*}/*.cs").Take(250)
            //var x = ExpandBraces("s");
            //var f2 = Globber.GetFiles(fileSystem.GetRootDirectory(), s);


            //NormalizedPath inputPath = rootPath.Combine("src").Combine("*.md,*.txt");
            //NormalizedPath docsInput = docsPath.Combine(@"{!output}*/**/");
            //@"./*{!.git,!bin,!obj,!packages,!*.Tests,}/{!.git,!bin,!obj,!packages,!*.Tests,}/*{!.git,!bin,!obj,!packages,!*.Tests,}/*.cs"
            //var cspath = srcPath.Combine(@"@""./*{!.git,!bin,!obj,!packages,!*.Tests,}/{!.git,!bin,!obj,!packages,!*.Tests,}/*{!.git,!bin,!obj,!packages,!*.Tests,}/*.cs""");  // Alongside the input folder
            //var cspath = srcPath.Combine(@"{!.git,!bin,!obj,!packages,!*.Tests,}/{!.git,!bin,!obj,!packages,!*.Tests,}/*{!.git,!bin,!obj,!packages,!*.Tests,}/*.cs");  // Alongside the input folder
            //var cspath = srcPath.Combine(@"../src/{!.vs,!obj,*}/{src,!bin,!*test*,!obj,*}/{src,!*test*,,!.vs,!bin,!obj,*}/*.cs");
            var cspath = @"../src/{!.vs,!obj,*}/{src,!bin,!*test*,!obj,*}/{src,!*test*,,!.vs,!bin,!obj,*}/*.cs";
            var csprojpath = @"../src/{!.vs,!obj,*}/{src,!bin,!*test*,!obj,*}/{src,!*test*,,!.vs,!bin,!obj,*}/*.csproj";
            var dllpath = @"../src/{!.vs,!obj,*}/{src,!bin,!*test*,!obj,*}/{src,!*test*,,!.vs,!bin,!obj,*}/*.dll";
            var slnpath = @"../src/{!.vs,!obj,*}/{src,!bin,!*test*,!obj,*}/{src,!*test*,,!.vs,!bin,!obj,*}/*.sln";

            //var x = fileSystem.GetInputFiles(cspath.ToString()).Take(250);

            var settings = new Dictionary<string, object>
            {
                //{ WebKeys.InputPaths, new PathCollection() { srcPath } },
                //{ WebKeys.InputPaths, new PathCollection(){  inputPath } },
            };

            //.AddInputPath($@"{srcPath.Combine("**/*.md")}")
            //NormalizedPath inputPath = rootPath.Combine("src");


            bootstrapper
                // Order of operations RunAsync, 1. Settings, 2. file system callback delegates.

                // Add all the configurations and settings.
                .AddDefaultConfigurationFiles()
                //.SetOutputPath(outputPath)
                .SetOutputPath(outputPath)
                // This needs to be set at this point to ensure the theme works correctly. Earlier or late fails silently.
                //  The themes path is in the app path by design as content.
                .SetThemePath(themePath)
                //.AddExcludedPath(fileSystem.RootPath.GetRelativePath(@"C:/Code/Repos/dotNetCollective/dotNetBlocks/tools/dotNetBlocks.docs/src/bin/Debug/net9.0"))

                .ConfigureFileSystem(
                    (fs, settings) =>
                        {
                            fs.TempPath = tempPath;
                            fs.CachePath = cachePath;
                        }
                )
                //.AddSetting(nameof(CleanMode), CleanMode.Self)
                .ConfigureSettings(
                    (settings, svcs, fs) =>
                    {
                        settings.Remove(DocsKeys.SourceFiles);
                        settings.Add(DocsKeys.SourceFiles, new string[] { cspath.ToString() });
                        settings.Remove(DocsKeys.ProjectFiles);
                        settings.Remove(DocsKeys.SolutionFiles);
                        settings.Remove(DocsKeys.AssemblyFiles);
                        settings.Add(DocsKeys.ProjectFiles, new string[] { csprojpath });
                        settings.Add(DocsKeys.SolutionFiles, new string[] {  });
                        settings.Add(DocsKeys.AssemblyFiles, new string[] { dllpath });
                    }
                )

                //.AddMappedInputPath(@$"{docsInput}", outputPath.Combine("docs"))
                //.AddMappedInputPath("docs/",outputPath.Combine("docs"))
                //.AddInputPath("docs")
                //.AddMappedInputPath(inputPath, "docs")
                //.AddInputPath(srcPath)
                //.AddExcludedPath("/*docable*")
                //.AddSourceFiles(sourceFiles.Select(f =>f.FullPath).ToArray())

                //.AddSourceFiles(sourceFiles.Cast<string>().ToArray())
                //.AddSetting(Statiq.Markdown.MarkdownKeys.MarkdownExtensions, "bootstrap")
                //.AddInputPath($@"{docsRoot}/**{{!output}}")
                //
                .AddDefaultLogging();
            //.SetFailureLogLevel(Microsoft.Extensions.Logging.LogLevel.Critical);

            return await bootstrapper.RunAsync();

            /// <summary>
            /// Finds the repo root.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            /// <exception cref="System.Exception">Could not find the root of the dot net blocks repo.</exception>
            NormalizedPath FindRepoRoot(string? path = default)
            {
                path ??= $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/";

                // Start the search at the executing directory.
                NormalizedPath appPath = path!;
                ;
                var folder = new DirectoryInfo(appPath.FullPath); // Load the executing folder.
                bool found = folder!.EnumerateFiles("Directory.Build.props").Any(); // Search for the props file in the current folder.
                while (!found)
                {
                    folder = folder.Parent; // move up to the parent folder
                    found = folder!.EnumerateFiles("Directory.Build.props").Any(); // search for the props file.
                }

                NormalizedPath repoRoot = $"{folder.FullName}";
                if (!folder.FullName!.EndsWith("dotNetBlocks")) throw new Exception("Could not find the root of the dot net blocks repo.");

                return repoRoot;
            }
        }

        //

        // src/**/{!.git,!bin,!obj,!packages,!*.Tests,}/**/*.cs
        //
        //
        internal static MethodInfo expandMethod = typeof(Globber).GetMethod("ExpandBraces", BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException("Cant find expandBraces");
        internal static IEnumerable<string> ExpandBraces(string pattern)
       => expandMethod.Invoke(null, new object[] { pattern }) as IEnumerable<string> ?? new string[] { };

    }

}