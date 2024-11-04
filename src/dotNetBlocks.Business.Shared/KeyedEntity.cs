using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Business.Shared
{


    /// <summary>
    /// class represents a <cref="BusinessEntity"/> with unique identifier.
    /// </summary>
    /// <seealso cref="dotNetBlocks.Business.Shared.BusinessEntity" />
    public abstract class KeyedEntity : BusinessEntity
    {

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is new; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsNew { get; }

    }


    /// <summary>
    /// <see cref="KeyedEntity"/> implementing an Id of type <typeparamref name="TId"/>
    /// </summary>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    /// <seealso cref="dotNetBlocks.Business.Shared.BusinessEntity" />
    public class KeyedEntity<TId> : KeyedEntity
        where TId : struct, IEquatable<TId> // TODO: Convertible, IComparable
    {

        public KeyedEntity(TId id) => Id = id;

        public virtual TId Id { get; set; } = default(TId);

        /// <see cref="KeyedEntity.IsNew" />
        /// <remarks>
        /// Keyed entities are new when their identifier has not been set.
        /// </remarks>
        public override bool IsNew => Convert.ToInt64(Id) == 0; // If we convert the id to an integer and its zero, its new for most numeric types.

        /// <summary>
        /// Creates new id.
        /// </summary>
        /// <value>
        /// a new identifier.
        /// </value>
        /// <remarks>
        /// Override this implementation for custom id generation strategies.</remarks>
        public virtual TId NewId => default(TId);
    }
    
    
}
