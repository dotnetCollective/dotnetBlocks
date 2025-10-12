using Acornima;
using dotNetBlocks.Docs.Toc;
using ICSharpCode.Decompiler.IL;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace dotNetBlocks.Docs.Markdown
{
    /// <summary>
    /// Works 
    /// </summary>
    public class FrontMatterService
    {

        private const string CommentStart = "<!";
        private const string CommentEnd = "->";

        private MarkdownPipeline _pipeline { get; init; }
        private IDeserializer _yamlDeserializer { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrontMatterService"/> class.
        /// </summary>
        public FrontMatterService()
        {
            // set up the markdig yaml processor. pipeline.
            // that extracts the html block.

            _pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .UseAdvancedExtensions()
                .Build();

            // Create the yaml front matter deserializer.

            _yamlDeserializer = new DeserializerBuilder()
            .Build();

        }



        /// <summary>
        /// Gets the front matter front a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// <see cref=" DocFrontMatter"/> from the file.
        /// </returns>
        /// <exception cref="System.IO.FileNotFoundException"> File does not exist.</exception>
        /// <exception cref="System.InvalidOperationException"> There was no front matter in the file.</exception>
        public async Task<DocFrontMatter> GetFrontMatterAsync(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"{fileName} does not exist");
            }



            // Read the file text.
            var fileText = await File.ReadAllTextAsync(fileName);

            // Parse the file.

            var document = Markdig.Markdown.Parse(fileText, _pipeline);


            // Files start with an HTML comment block containing the front matter.

            // Get the HTML comment block with the front matter.
            var header = document.Descendants<HtmlBlock>().FirstOrDefault();

            if (header is null || header.Type != HtmlBlockType.Comment)
                throw new InvalidOperationException($"{fileName} missing front matter block. ");

            var headerText = new StringBuilder(); // Unwrap the front matter for parsing.
            for (var lineIndex = 0; lineIndex < header.Lines.Count; lineIndex++)
            {
                var line = header.Lines.Lines[lineIndex];

                if (line.Slice.IsEmpty) // Skip empty lines.
                {
                    continue;
                }

                string? lineText = line.Slice.ToString();
                if (lineText == null)
                    continue;

                if (lineText.StartsWith(CommentStart)) // Trim "<!"
                {
                    headerText.AppendLine(lineText.Substring(CommentStart.Length));
                    continue;
                }
                if (lineText.EndsWith(CommentEnd)) // Trime "->"
                {
                    headerText.AppendLine(lineText.Replace(CommentEnd, "-"));
                    continue;
                }

                headerText.AppendLine(lineText);
            }

            // Parse the markup that was contained in the html comment.
            var headerDocument = Markdig.Markdown.Parse(headerText.ToString(), _pipeline);

            var frontMatterBlock = headerDocument.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

            if (frontMatterBlock is null)
                throw new InvalidOperationException($"{fileName} invalid header information.");


            try
            {
                var docFrontMatter = _yamlDeserializer.Deserialize<DocFrontMatter>(frontMatterBlock.Lines.ToString());

                return docFrontMatter;
            }
            catch (YamlException e)
            {
                // Pack the front matter and file name into an exception.
                throw new Exception($"{fileName} yaml deserialization error {e.Message}", e);
            }
        }
    }
}