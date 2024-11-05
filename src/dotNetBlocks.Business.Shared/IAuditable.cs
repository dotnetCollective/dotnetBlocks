using System;

namespace dotNetBlocks
{
    /// <summary>
    /// Interface for _auditProperties objects and entities.
    /// </summary>
    /// <remarks> Standard Properties for tracking item CRUD behaviors.</remarks>
    public interface IAuditable
    {
        /// <summary>
        /// Date time created;
        /// </summary>
        /// <value>
        /// time created
        /// </value>
        DateTimeOffset? CreatedAt { get; set; }
        /// <summary>
        /// creator
        /// </summary>
        /// <value>
        /// Creator.
        /// </value>
        string? CreatedBy { get; set; }
        /// <summary>
        /// date time updated.
        /// </summary>
        /// <value>
        /// Date time of the last update.
        /// </value>
        DateTimeOffset? UpdatedAt { get; set; }
        /// <summary>
        /// Gets or sets the updated by.
        /// </summary>
        /// <value>
        /// Update actor.
        /// </value>
        string? UpdatedBy { get; set; }
    }
}
