using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Docs.Toc
{
    /// <summary>
    /// structure for docFX node in TOC yaml file.
    /// </summary>
    [DebuggerDisplay("{Name},{Href}")]
    public class TocNode
    {

        /// <summary>
        /// Toc entry Name
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [YamlDotNet.Serialization.YamlMemberAttribute( Alias = "name")]        
        public string? Name { get; set; }

        /// <summary>
        /// TOC Href pointing to file.
        /// </summary>
        /// <value>
        /// The href.
        /// </value>
        [YamlDotNet.Serialization.YamlMemberAttribute(Alias = "href")]
        public string? Href { get; set; }

        [YamlDotNet.Serialization.YamlMemberAttribute(Alias = "order")]
        public int? Order { get; set; }

        /// <summary>
        /// <see cref="TocNode"/> siblinges and children for this entry.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [YamlDotNet.Serialization.YamlMemberAttribute(Alias = "items")]
        public IEnumerable<TocNode>? Items { get; set; }
    }
}
