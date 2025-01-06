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

            // Start the search at the executing directory.
            NormalizedPath appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
;

            var repoRoot = FindRepoRoot(appPath.FullPath);

             //repoRoot = $"{repoRoot}{NormalizedPath.Slash}";

            // Set all the required paths.
            var docsRoot = repoRoot.Combine("docs");
            var srcRoot = repoRoot.Combine("src");
            var tempPath = appPath.Combine("temp");
                var cachePath = appPath.Combine("cache");
            var outputPath = docsRoot.Combine("output");

            //args = new string[] { "serve" };
            //args = new string[] { "-l Debug", "run serve", };

            //args = new string[] { "-l debug"};

            //args = new string[] { "glob", "eval", @"\src/**/!.git,!bin,!obj,!packages,!*.Tests,/**/*.cs", @"C:\Code\Repos\dotNetCollective\dotNetBlocks\" };
            //args = new string[] { @"glob eval"};

            //args = new string[] { "-- preview" };
            args = new string[] { "preview" };
            //args = new string[] { "-- loglevel", "information", " -- preview" };
            // Ensure consistent date handling

            var sourceFiles = new NormalizedPath[]
                            {
            //"../src/**/{!.git,!bin,!obj,!packages,!*.Tests,}/**/*.cs", // Alongside the input folder
            @$"{repoRoot}/src/**{{!tools,!docs,!output,,!*test*,!*.docs*}}/{{!.git,!bin,!obj,!packages,!*test*,!*.Tests,}}/**/*.cs", // Alongside the input folder
            //"../../src/**/{!.git,!bin,!obj,!packages,!*.Tests,}}}/**/*.cs" // Alongside the parent project folder
            };

            var settings = new Dictionary<string, object>
            {
                { DocsKeys.ApiPath, "api" },
                { WebKeys.TempPath, tempPath },
                { WebKeys.CachePath, cachePath },
                { WebKeys.OutputPath, outputPath },
                //{ DocsKeys.SourceFiles, sourceFiles },
                ////{ WebKeys.InputPaths, new NormalizedPath[]{ @"input",@"" } },
                { WebKeys.InputPaths, new NormalizedPath[]{ $"{docsRoot}{{!output}}" } },
                //{ WebKeys.ThemePaths, new NormalizedPath[] { @$"themes/docable/" } }, // Does not work for this setup.
                //{ WebKeys.Title, "dotNetBlocks API" },
            };


            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            var bootstrapper = Bootstrapper.Factory.CreateDocs(args);

            bootstrapper
                .AddInputPath($@"{docsRoot}/**{{!output}}/*.*")
                .AddDefaultConfigurationFiles()
                .AddSettings(settings)


                // This needs to be set at this point to ensure the theme works correctly. Earlier or late fails silently.
                .SetThemePath((NormalizedPath)"themes/docable/")
                //.AddSourceFiles(sourceFiles.Cast<string>().ToArray())
                //.AddSetting(Statiq.Markdown.MarkdownKeys.MarkdownExtensions, "bootstrap")
                //.AddInputPath($@"{docsRoot}/**{{!output}}")
                //
                .SetFailureLogLevel(Microsoft.Extensions.Logging.LogLevel.None);


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
    }
}