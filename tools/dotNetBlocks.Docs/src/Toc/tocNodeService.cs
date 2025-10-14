using dotNetBlocks.Docs.Markdown;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace dotNetBlocks.Docs.Toc
{
    internal class tocNodeService
    {

        private Lazy<FrontMatterService> _frontMatterService = new Lazy<FrontMatterService>();
        private FrontMatterService FrontMatterService => _frontMatterService.Value;

        private Lazy<ISerializer> _yamlSerializer = new Lazy<ISerializer>
            (() => new SerializerBuilder()
                .WithIndentedSequences()
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
                .Build());

        private ISerializer yamlSerializer => _yamlSerializer.Value;

        /// <summary>
        /// Gets a table of contents node for the specified file.
        /// </summary>
        /// <param name="fileName">The path or name of the file for which to retrieve the table of contents node. Cannot be null.</param>
        /// <returns>
        /// a <see cref="TocNode"/> for the
        /// specified file.
        /// </returns>
        public async Task<TocNode> GetFileTocNodeAsync([NotNull] string? fileName)
        {
            ArgumentNullException.ThrowIfNull(fileName);

            return await Task.FromResult(
                BuildTocNode(
                    await FrontMatterService.GetFrontMatterAsync(fileName)
                    , fileName)
                );
        }

        /// <summary>
        /// Creates a new table of contents (TOC) node using the specified <see cref="DocFrontMatter"/>and file name.
        /// </summary>
        /// <param name="frontMatter">The front matter metadata for the document, containing the title and order information used to populate the TOC
        /// node.</param>
        /// <param name="fileName">The file name of the document. The file name is used as the hyperlink reference (Href) and as the node name if
        /// the title is not specified.</param>
        /// <returns>A <see cref="TocNode"/> instance representing the document in the table of contents.</returns>
        private TocNode BuildTocNode(DocFrontMatter frontMatter, string fileName)
        {
            var shortfileName = Path.GetFileName(fileName);

            return new TocNode()
            {
                Name = frontMatter.Title ?? shortfileName,
                Href = shortfileName,
                Order = frontMatter.Order,
            };
        }

        public async Task WriteTocFileAsync(string filename, TocNode toc, bool overWrite = false)
        {
            if (!overWrite && File.Exists(filename))
                throw new Exception($"{filename} already exists could not write file.");

            using (var writer = File.CreateText(filename))
            {
                yamlSerializer.Serialize(writer, toc);
                await Task.CompletedTask;
            }
        }
    }
}