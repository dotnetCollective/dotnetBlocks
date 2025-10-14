using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Docs.Toc
{
    /// <summary>
    /// Toc callback action <see cref="TocFolderDecision"/>
    /// </summary>
    /// <remarks>
    /// <see cref="TocFolderDecision"/> result deciding what to do with a folder during toc generations
    /// </remarks>
    [Flags]
    public enum  TocActions
    {
        /// <summary>
        /// Ignores the folder.
        /// </summary>
        Ignore = 0x00,
        /// <summary>
        /// Process the folder
        /// </summary>
        Process = 0x01, 
        /// <summary>
        /// Write Toc.yaml file if missing
        /// </summary>
        WriteToc = 0x02,
        /// <summary>
        /// Overwrite toc.yaml file if it exists
        /// </summary>
        Overwrite = 0x04,
        /// <summary>
        /// Creates a nested TOC node
        /// </summary>
        /// <remarks>
        /// href = folder/toc.yml
        /// </remarks>
        NestedToc = 0x100,
        /// <summary>
        /// Creates a referenced TOC node
        /// </summary>
        /// <remarks>
        /// href = folder/
        /// </remarks>
        ReferencedToc = 0x200,

        /// <summary>
        /// Sorts the nodes by ascending order.
        /// Lowest value nodes come first.
        /// </summary>
        SortNodes = 0x400,
        // SortNodesAscending = 0x800,

    }
}
