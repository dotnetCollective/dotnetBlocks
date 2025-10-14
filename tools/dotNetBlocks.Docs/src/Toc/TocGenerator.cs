using Docfx.YamlSerialization;
using dotNetBlocks.Docs.Markdown;
using ICSharpCode.Decompiler.Util;
using Microsoft.AspNetCore.Builder;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using YamlDotNet.Serialization;

namespace dotNetBlocks.Docs.Toc;

/// <summary>
/// Signature for a method delegate to decide the actions to perform for a specified folder.
/// </summary>
/// <param name="folderName"> folder being processed</param>
/// <param name="title">title to use for the entry. </param>
/// <param name="order"> Overrides the order value for the node.</param>
/// <returns><see cref="TocActions"/> to take on the entry. </returns>
public delegate TocActions TocFolderDecision(string? folderName, out string? title, out int? order);

/// <summary>
/// Generates Table of Contents Files.
/// </summary>
public class TocGenerator
{
    /// <summary>
    /// Gets the default action to take for a table of contents (TOC) folder when no custom decision logic is required.
    /// </summary>
    /// <remarks>This property provides a default delegate that determines how TOC folders are processed. It
    /// can be used as a fallback when no specific folder decision logic is supplied by the caller.</remarks>
    public static TocFolderDecision DefaultTocDecision { get; } = (string? folderName, out string? title, out int? order ) => { title = default; order = default; return TocActions.Ignore; }; // Do nothing.

    private const string TOC_FILE_NAME = "toc.yml";
    private const string INDEX_FILE_NAME = "index.md";


    private Lazy<tocNodeService> _nodeService = new Lazy<tocNodeService>();
    private tocNodeService NodeService => _nodeService.Value;


    /// <summary>
    /// Builds the table of contents (TOC) tree starting from the specified root folder.
    /// </summary>
    /// <param name="rootFolder">The path to the root folder from which to build the table of contents. The folder must exist.</param>
    /// <param name="folderDecision">An optional delegate that determines how folders are included in the TOC. If not specified, a default decision
    /// logic is used.</param>
    /// <returns> <see cref="TocNode"/> root node and all the nodes in the generated TOC tree.
    /// .</returns>
    /// <remarks>
    /// Builds the toc structures as referenced in this docfx doc (https://dotnet.github.io/docfx/docs/table-of-contents.html)
    /// </remarks>
    /// 

    public async Task<TocNode> BuildTocAsync(string rootFolder, TocFolderDecision? folderDecision = default)
    {
        folderDecision ??= DefaultTocDecision;
        CheckFolderExists(rootFolder);

        return await BuildFolderTocAsync(rootFolder, default, folderDecision);
    }


    /// <summary>
    /// Builds Table of Contents for a folder.
    /// </summary>
    /// <param name="folderName">Name of the folder.</param>
    /// <param name="relativeFolderName">Name of the relative folder.</param>
    /// <param name="folderDecision">The folder decision.</param>
    /// <returns></returns>
    /// <exception cref="System.InvalidOperationException"></exception>
    /// <remarks>
    /// Documents the files in the folder and every folder.
    /// Builds the toc for the folder and returns the node as a child to the parent.
    /// </remarks>
    public async Task<TocNode> BuildFolderTocAsync([NotNull] string? folderName, string? relativeFolderName, TocFolderDecision? folderDecision = default)
    {
        // Set default value if not set.
        folderDecision ??= DefaultTocDecision;

        CheckFolderExists(folderName);

        // Get a decision for this folder.
        var action = folderDecision(folderName, out var newTitle, out var order);

        if (action.HasFlag(TocActions.ReferencedToc) && action.HasFlag(TocActions.NestedToc)) throw new InvalidOperationException($"Action cannot specify referenced and nested TOC flags.");

        // Parent should not call this method if the action is ignore.
        if (action == TocActions.Ignore) throw new InvalidOperationException($"{nameof(BuildFolderTocAsync)} should not be called with {nameof(TocActions.Ignore)} action.");

        // Check if there is a toc file.
        var tocFileName = Path.Combine(folderName, TOC_FILE_NAME);
        var tocExists = File.Exists(tocFileName);
        var indexFileName = Path.Combine(folderName, INDEX_FILE_NAME);
        var indexFileExists = File.Exists(indexFileName);

        // Process all the files in our folder.
        var toc = await GetFolderFileContentAsync(folderName);

        // Prepare the items list for sub folder processing.
        var tocItems = toc.Items as List<TocNode> ?? new List<TocNode>();

        toc.Items = tocItems;

        // Get a node for each child subFolderName.
        foreach (var subFolderName in Directory.GetDirectories(folderName))
        {
            // Do we process this folder?
            if (folderDecision(subFolderName, out _, out _) == TocActions.Ignore)
                continue;

            // Process the sub folder.
            tocItems.Add(
                await BuildFolderTocAsync(
                    subFolderName,
                    Path.GetRelativePath(folderName, subFolderName), folderDecision)
                    );
        }

        // TODO: Add sorting rules based on natural sort, sort order and special files like index.md and toc.yml.
        if (action.HasFlag(TocActions.SortNodes))
        {
            tocItems.Sort(
                (x, y) =>(x.Order??0).CompareTo(y.Order??0)
                );
        }


        // We have all our file content and our sub-folder content.
        // Write the toc as the parent referring to all the items.

        // The TOC content is all items.
        if (action.HasFlag(TocActions.WriteToc))
        {
            if (!tocExists || action.HasFlag(TocActions.Overwrite))
            {
                // OVerwrite the toc.
                await NodeService.WriteTocFileAsync(tocFileName, toc, true);

                tocExists = true;
            }
        }

        // Prepare the results for the parent - populate the root node info relative to the parent.
        // Out children are only important to get our index file and populate out display information.

        // 

        // Get the href rooted properly.
        if (!string.IsNullOrWhiteSpace(relativeFolderName))
            toc.Href = $"{relativeFolderName}/";

        toc.Name ??= newTitle;


        // Re-read the index file if it exists.
        if (indexFileExists)
        {
            // Find the index file node.
            var indexFileNode =
                (from i in toc.Items
                 where i.Href?.Contains("index.md", StringComparison.OrdinalIgnoreCase) ?? false
                 select i).FirstOrDefault() ?? // If there is no match, read the file.
                await NodeService.GetFileTocNodeAsync(indexFileName);

            if (indexFileNode is null) throw new NullReferenceException(nameof(indexFileNode));

            // Update the node info.
            toc.Name = newTitle ?? indexFileNode.Name ?? relativeFolderName; // Decision file name or fallback on index file;
            toc.Order = order ?? indexFileNode.Order;
        }
        else
        {
            // Set the properties based on the action values.
            // Update the node info.
            toc.Name = toc.Name ??  newTitle ?? relativeFolderName; // Decision file name or fallback on index file;
            toc.Order = toc.Order ??  order;
        }

        // Href is good, now point to toc or index file.
        if (tocExists)
        {
            if (action.HasFlag(TocActions.NestedToc))
            {
                // Nested TOC point to sub folder TOC.
                // href=folder/toc.yml.
                toc.Href += TOC_FILE_NAME;
            }
            else
            {
                // Default is referenced href=folder/
            }
        }
        else // TOC file does not exist, so point to content.
        if (indexFileExists)
        {
            toc.Href += INDEX_FILE_NAME;
        }

        // Clear out the children before returning this node.
        toc.Items = null;

            // Return this full node to the parent.
            return toc;

    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="folderName"></param>
    /// <returns></returns>
    public async Task<TocNode> GetFolderFileContentAsync(string? folderName) 
    { 
        CheckFolderExists(folderName);

        // Create the node for this subFolderName and all the files in it.
        TocNode folderToc = new TocNode();
        var folderItems = new List<TocNode>();
        folderToc.Items = folderItems;

        // GetFrontMatterAsync is an async method, so we can't use a linq query.

        // process the markdown files in the subFolderName.
        foreach (var fileName in Directory.GetFiles(folderName, "*.md"))
        {
            folderItems.Add(
                await NodeService.GetFileTocNodeAsync(fileName)
                );
        };

        return folderToc;

    }


    /// <summary>
    /// Checks whether the specified folderName exists and throws an exception if it does not.
    /// </summary>
    /// <param name="folderName">The path of the folderName to check. Cannot be null or empty.</param>
    /// <exception cref="DirectoryNotFoundException">Thrown if a folderName with the specified path does not exist.</exception>
    private void CheckFolderExists([NotNullAttribute] string? folderName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(folderName, nameof(folderName));

        if (!Directory.Exists(folderName)) throw new DirectoryNotFoundException(folderName);

    }
}
    