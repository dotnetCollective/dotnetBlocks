using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Business.Shared
{
    /// <summary>
    /// Tracks the audit properties for a class.
    /// </summary>
    /// <seealso cref="dotNetBlocks.IAuditable" />
    public class AuditProperties : IAuditable
    {
        /// <see cref=" IAuditable.CreatedAt"/>
        public DateTimeOffset? CreatedAt { get; set; }

        /// <see cref=" IAuditable.CreatedBy"/>
        public string? CreatedBy { get; set; }

        /// <see cref=" IAuditable.UpdatedAt"/>
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <see cref=" IAuditable.UpdatedBy"/>
        public string? UpdatedBy { get; set; }
    }
}
