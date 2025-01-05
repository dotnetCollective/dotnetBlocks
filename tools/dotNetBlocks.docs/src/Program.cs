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
            NormalizedPath appPath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/"
;
            var folder = new DirectoryInfo(appPath.FullPath);
            bool found = folder!.EnumerateFiles("Directory.Build.props").Any();
            while (!found)
            {
                folder = folder.Parent;
                found = folder!.EnumerateFiles("Directory.Build.props").Any();
            }

            NormalizedPath repoRoot = $"{folder.FullName}";
            if (!folder.FullName!.EndsWith("dotNetBlocks")) throw new Exception("Could not find the root of the dot net blocks repo.");

            repoRoot = $"{repoRoot}/";

            // Set the docs root where we want output to go to.
            NormalizedPath docsRoot = repoRoot.Combine("docs");
            NormalizedPath srcRoot = repoRoot.Combine("src");

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
                //{ DocsKeys.ApiPath, "api" },
                { WebKeys.TempPath, (NormalizedPath) appPath.Combine("temp") },
                { WebKeys.CachePath, (NormalizedPath) appPath.Combine("cache") },
                { WebKeys.OutputPath, (NormalizedPath)docsRoot.Combine("output")},
                { DocsKeys.SourceFiles, sourceFiles },
                //{ WebKeys.InputPaths, (NormalizedPath) @$"{docsRoot}input" },
                //{ WebKeys.InputPaths, new NormalizedPath[]{ @"input",@"" } },
                //{ WebKeys.ThemePaths, new NormalizedPath[] { @$"themes/docable/" } }, // Does not work for this setup.
                //{ WebKeys.Title, "dotNetBlocks API" },
            };


            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            return await Bootstrapper.Factory
                .CreateDocs(args) 
                .AddDefaultConfigurationFiles()
                .AddSettings(settings)
                .ConfigureSettings(
                    (settings) => {
                    //settings[DocsKeys.SourceFiles] = new string[]{};
                    }
                )
                // This needs to be set at this point to ensure the theme works correctly. Earlier or late fails silently.
                .SetThemePath((NormalizedPath) "themes/docable/")
                //.AddSourceFiles(sourceFiles.Cast<string>().ToArray())
                .AddSetting(Statiq.Markdown.MarkdownKeys.MarkdownExtensions, "bootstrap")
                .AddInputPath($@"{docsRoot}/**{{!output}}")
                //
                .SetFailureLogLevel(Microsoft.Extensions.Logging.LogLevel.None)

                .RunAsync();
        }
    }
}
