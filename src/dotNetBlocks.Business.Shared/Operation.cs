using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Business.Shared
{
    /// <summary>
    /// operation contains all the information required to execute a business operation in the framework.
    /// </summary>
    /// <remarks>
    /// Operations are similar to a command patter, but are not active. They contain data only and are not active except for self-validation.
    /// </remarks>
    public class Operation
    {

        /// <summary>
        /// Gets or sets the transaction identifier.
        /// </summary>
        /// 
        /// <remarks>
        /// Identfies the current transaction this operation is a part of.
        /// </remarks>
        /// <value>
        /// The transaction identifier.
        /// </value>
        public Guid TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the identifier for this operation.
        /// </summary>
        /// <value>
        /// The operation identifier.
        /// </value>
        public Guid OperationId { get; set; }

        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        /// <value>
        /// The correlation identifier.
        /// </value>
        /// <remarks>
        /// Set at the initiation of a multiple event process to connect diagnostics and reporting for an entire process chain.
        /// </remarks>
        public Guid CorrelationId { get; set; }

    }

    /// <summary>
    /// operation containing <see cref="BusinessEntity"/> required to execute the business operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class Operation<TEntity> : Operation
        where TEntity : BusinessEntity
    {

        /// <summary>
        /// Gets or sets the entity used in the operation.
        /// </summary>
        /// <value>
        /// The entity to use in the operation.
        /// </value>
        public TEntity? Entity { get; set; }

    }
}
