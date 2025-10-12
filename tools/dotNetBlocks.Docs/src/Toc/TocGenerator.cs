using Docfx.YamlSerialization;
using dotNetBlocks.Docs.Markdown;
using Microsoft.AspNetCore.Builder;
using System;
using System.Diagnostics.CodeAnalysis;
using YamlDotNet.Serialization;

namespace dotNetBlocks.Docs.Toc;

/// <summary>
/// Signature for a method delegate to decide the actions to perform for a specified folder.
/// </summary>
/// <param name="folderName"> folder being processed</param>
/// <param name="Title">Title to use for the entry. </param>
/// <returns><see cref="TocActions"/> to take on the entry. </returns>
public delegate TocActions TocFolderDecision(string? folderName, out string? Title);

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
    public static TocFolderDecision DefaultTocDecision { get; } = (string? folderName, out string? title) => { title = default; return TocActions.Process; };

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
    public async Task<TocNode> BuildTocAsync(string rootFolder, TocFolderDecision? folderDecision = default)
    {
        folderDecision ??= DefaultTocDecision;
        CheckFolderExists(rootFolder);

        return await BuildFolderTocAsync(rootFolder, default, folderDecision);
    }

    
    public async Task<TocNode> BuildFolderTocAsync([NotNull] string? folderName, string? relativeFolderName, TocFolderDecision? folderDecision = default)
    {
        // Set default value if not set.
        folderDecision ??= DefaultTocDecision;

        CheckFolderExists(folderName);

        // Get a decision for this folder.
        var action = folderDecision(folderName, out var newTitle);

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
            if (folderDecision(subFolderName, out _) == TocActions.Ignore)
                continue;

            // Process the sub folder.
            tocItems.Add(
                await BuildFolderTocAsync(
                    subFolderName,
                    Path.GetRelativePath(folderName, subFolderName), folderDecision)
                    );
        }

        // We have all our file content and our sub-folder content.
        // Write the toc as the parent referring to all the items.
        if (action == TocActions.WriteToc)
        {
            if (!tocExists || action.HasFlag(TocActions.Overwrite))
            {
                // OVerwrite the toc.
                await NodeService.WriteTocFileAsync(tocFileName, toc, true);

                tocExists = true;
            }
        }

        // Prepare the results for the parent - populate the root node info relative to the parent.

        // Get the href rooted properly.
        if (!string.IsNullOrWhiteSpace(relativeFolderName))
            toc.Href = $"{relativeFolderName}/";

        toc.Name ??= newTitle;

        // Re-read the index file if it exists.
        // We could try find the index but may get the wrong one.
        if (indexFileExists)
        {
            var indexFileNode = await NodeService.GetFileTocNodeAsync(indexFileName);

            // Update the node info.
            toc.Name = newTitle ?? indexFileNode.Name ?? relativeFolderName?.ToUpper(); // Decision file name or fallback on index file;
            toc.Order = indexFileNode.Order;
        }

        // Href is good, now point to toc or index file.
        if (tocExists)
            toc.Href += TOC_FILE_NAME;
        else if (indexFileExists)
        {
            toc.Href += INDEX_FILE_NAME;
        }


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
    