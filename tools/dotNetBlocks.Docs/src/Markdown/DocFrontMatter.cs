using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Docs.Markdown
{
    /// <summary>
    /// Document Front MAtter structure
    /// </summary>
    public class DocFrontMatter
    {
        /// <summary>
        /// The Document Title.
        /// </summary>
        public string Title = string.Empty;

        /// <summary>
        /// The Document sub title.
        /// </summary>
        public string? SubTitle = default;

        /// <summary>
        /// Document description.
        /// </summary>
        public string? Description = default;
        /// <summary>
        /// The navigation title
        /// </summary>
        public string? NavigationTitle = default;
        /// <summary>
        /// The breadcrumb title
        /// </summary>
        public string? BreadcrumbTitle = default;
        /// <summary>
        /// The show in navigation
        /// </summary>
        public bool? ShowInNavigation = default;
        /// <summary>
        /// The show in sidebar
        /// </summary>
        public bool? ShowInSidebar = default;
        /// <summary>
        /// The no sidebar
        /// </summary>
        public bool? NoSidebar = default;
        /// <summary>
        /// The excerpt
        /// </summary>
        public string? Excerpt = default;
        /// <summary>
        /// The level
        /// </summary>
        public int? Level = default;
        /// <summary>
        /// The order
        /// </summary>
        public int? Order = default;
        /// <summary>
        /// The document identifier.
        /// </summary>
        public string? href = default;

    }
}
