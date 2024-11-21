using RT.Comb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Business.Shared
{
    /// <summary>
    /// Represents all business activities in the system.
    /// </summary>
    /// <remarks>
    /// common activities:
    /// <see cref="Operation"/>
    /// </remarks>
    public class BusinessActivity
    {
        /// <summary>
        /// Unique id for the business activity.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; } = RT.Comb.Provider.Legacy.Create();

        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        /// <value>
        /// The correlation identifier.
        /// </value>
        /// <remarks>
        /// Set at the initiation of a multiple event process to connect diagnostics and reporting for an entire process chain.
        /// </remarks>
        public Guid CorrelationId { get; set; } = RT.Comb.Provider.Legacy.Create();

        /// <summary>
        /// Gets or sets the transaction identifier.
        /// </summary>
        /// <value>
        /// The transaction identifier.
        /// </value>
        public Guid TransactionId { get; set; } = RT.Comb.Provider.Legacy.Create();

        /// <summary>
        /// Gets or sets the related to identifier.
        /// </summary>
        /// <value>
        /// The related to identifier.
        /// </value>
        public Guid RelatedToId { get; set; } = default;

        public DateTimeOffset CreatedAt { get; set; } = default;

        /// <summary>
        /// Default constructor for <see cref="BusinessActivity"/> class with default values.
        /// Initializes a new instance of the <see cref="BusinessActivity"/> class.
        /// </summary>
        public BusinessActivity() : base() { }


        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessActivity"/> class.with values transferred from <paramref name="relatedTo"/> object provided.
        /// </summary>
        /// <param name="relatedTo">The related to.</param>
        public BusinessActivity(BusinessActivity relatedTo) : base()
        {
            if (relatedTo == null)
                return;

            RelatedToId = relatedTo.Id;
            CorrelationId = relatedTo.CorrelationId;
            TransactionId = relatedTo.TransactionId;
        }

    }

    public class BusinessActivity<TEntity> : BusinessActivity
        where TEntity : BusinessEntity, new()
    {
        public BusinessActivity()
        {
        }

        public BusinessActivity(BusinessActivity relatedTo) : base(relatedTo)
        {
        }

        public BusinessEntity? Entity { get; set; }

    }

}
